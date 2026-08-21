/*
    Este arquivo concentra o comportamento do formulário de produto.

    Ele controla:
    - criação e edição;
    - cancelamento da edição;
    - comportamento do campo de medida;
    - busca de produtos semelhantes;
    - preenchimento dos campos durante a edição.

    As chamadas HTTP são recebidas por funções externas.
    Assim, este módulo não precisa conhecer detalhes de fetch ou endpoints.
*/


/**
 * Cria o objeto de configuração do formulário.
 *
 * O app.js fornece os elementos HTML e as funções responsáveis pela API,
 * pela interface e pelo carregamento da lista.
 *
 * @param {object} options Dependências usadas pelo formulário.
 * @returns {object} Funções que serão usadas pelo app.js.
 */
export function createProductFormController({
    elements,
    api,
    ui,
    reloadProducts
}) {
    const {
        productForm,
        productIdInput,
        formTitle,
        submitButton,
        cancelButton,
        formMessage,
        nameInput,
        similarProductsContainer,
        descriptionInput,
        priceInput,
        categoryInput,
        brandNameInput,
        weightOrVolumeInput,
        weightOrVolumeLabel,
        measureTypeInput
    } = elements;

    /*
        Identificador do timer de debounce.

        Enquanto a pessoa digita, cancelamos o timer anterior e iniciamos outro.
        A busca só será feita quando ela passar 350 ms sem digitar novamente.
    */
    let similarProductsDebounceTimer = null;

    /*
        Número sequencial usado para evitar que uma resposta antiga sobrescreva
        resultados de uma busca mais recente.
    */
    let similarProductsSearchRequestId = 0;

    // Registra os eventos dos campos e botões do formulário.
    productForm.addEventListener("submit", submitProductForm);
    nameInput.addEventListener("input", handleSimilarProductsSearch);
    measureTypeInput.addEventListener("change", updateMeasurementField);
    cancelButton.addEventListener("click", cancelEdit);

    // Configura o estado inicial do campo de medida.
    updateMeasurementField();

    return {
        editProduct,
        resetProductForm,
        cancelEdit,
        updateMeasurementField
    };


    /**
     * Ajusta o rótulo e o estado do valor da medida conforme o MeasureType.
     *
     * 0 = Unit       -> rótulo "Unidade", valor fixo 1 e campo bloqueado.
     * 1 = Kilogram   -> rótulo "Peso" e campo editável.
     * 2 = Gram       -> rótulo "Peso" e campo editável.
     * 3 = Liter      -> rótulo "Volume" e campo editável.
     * 4 = Milliliter -> rótulo "Volume" e campo editável.
     */
    function updateMeasurementField() {
        const measureType = Number(measureTypeInput.value);

        /*
            Enquanto nenhuma opção foi escolhida, mantemos o rótulo genérico
            e deixamos o campo editável para não causar comportamento inesperado.
        */
        if (measureTypeInput.value === "") {
            weightOrVolumeLabel.textContent = "Peso ou Volume";
            weightOrVolumeInput.disabled = false;
            weightOrVolumeInput.required = true;
            return;
        }

        if (measureType === 0) {
            /*
                Unidade sempre equivale a 1.

                O campo fica bloqueado para evitar valores como
                2,5 Unidades.
            */
            weightOrVolumeLabel.textContent = "Unidade";
            weightOrVolumeInput.value = 1;
            weightOrVolumeInput.disabled = true;

            /*
                Como o campo está desabilitado, ele não participa da validação
                nativa do formulário. O valor 1 será adicionado ao payload
                pelo JavaScript.
            */
            weightOrVolumeInput.required = false;
            return;
        }

        /*
            Para as demais medidas, o usuário pode informar um valor decimal.
            Se ele veio de Unidade, liberamos o campo novamente.
        */
        weightOrVolumeInput.disabled = false;
        weightOrVolumeInput.required = true;

        if (measureType === 1 || measureType === 2) {
            weightOrVolumeLabel.textContent = "Peso";
            return;
        }

        if (measureType === 3 || measureType === 4) {
            weightOrVolumeLabel.textContent = "Volume";
            return;
        }

        // Proteção para qualquer valor inesperado.
        weightOrVolumeLabel.textContent = "Peso ou Volume";
    }


    /**
     * Controla a busca de produtos semelhantes enquanto o nome é digitado.
     *
     * Esta função roda a cada caractere digitado, mas não chama a API
     * imediatamente. Ela inicia um debounce de 350 ms.
     */
    function handleSimilarProductsSearch() {
        // trim evita buscas para texto composto apenas por espaços.
        const typedName = nameInput.value.trim();

        // Cancela o timer da digitação anterior, se existir.
        clearTimeout(similarProductsDebounceTimer);

        /*
            Durante a edição, não precisamos mostrar produtos semelhantes,
            pois o formulário já está carregando um produto existente.
        */
        if (productIdInput.value) {
            clearSimilarProducts();
            return;
        }

        /*
            Se houver menos de 3 caracteres, não pesquisamos.
            Também limpamos a lista para não mostrar resultados antigos.
        */
        if (typedName.length < 3) {
            clearSimilarProducts();
            return;
        }

        // Espera 350 ms sem nova digitação antes de chamar a API.
        similarProductsDebounceTimer = setTimeout(() => {
            searchSimilarProducts(typedName);
        }, 350);
    }


    /**
     * Consulta produtos com nomes semelhantes.
     *
     * A função de API foi recebida pelo app.js para manter este módulo
     * independente da implementação de fetch.
     *
     * @param {string} name Nome digitado no formulário.
     */
    async function searchSimilarProducts(name) {
        /*
            Aumenta o identificador para marcar esta como a busca mais recente.
            Se outra busca começar depois, esta resposta deixará de ser atual.
        */
        const currentRequestId = ++similarProductsSearchRequestId;

        try {
            const products = await api.searchProductsByName(name);

            /*
                Se uma nova busca começou enquanto aguardávamos esta resposta,
                ignoramos o resultado antigo.
            */
            if (currentRequestId !== similarProductsSearchRequestId) {
                return;
            }

            /*
                A pessoa pode ter apagado ou alterado o nome enquanto aguardávamos.
                Só mostramos dados se o valor atual ainda corresponder à busca.
            */
            if (nameInput.value.trim() !== name) {
                return;
            }

            renderSimilarProducts(products);
        } catch (error) {
            console.error(error);

            /*
                A busca semelhante é apenas uma ajuda visual.
                Por isso, um erro nela não bloqueia o cadastro.
            */
            clearSimilarProducts();
        }
    }


    /**
     * Desenha a lista de produtos semelhantes abaixo do campo Nome.
     *
     * @param {Array} products Produtos retornados pela API.
     */
    function renderSimilarProducts(products) {
        if (!Array.isArray(products) || products.length === 0) {
            clearSimilarProducts();
            return;
        }

        similarProductsContainer.innerHTML = `
            <p class="similar-products-title">
                Produtos semelhantes já cadastrados:
            </p>

            <ul class="similar-products-list">
                ${products
                .map((product) => {
                    return `
                            <li>
                                <strong>${ui.escapeHtml(product.name)}</strong>
                                <span>SKU: ${ui.escapeHtml(product.sku)}</span>
                            </li>
                        `;
                })
                .join("")}
            </ul>
        `;
    }


    /**
     * Limpa e oculta a área de produtos semelhantes.
     */
    function clearSimilarProducts() {
        similarProductsContainer.innerHTML = "";
    }


    /**
     * Envia o formulário para criar ou editar um produto.
     *
     * O comportamento depende do campo oculto product-id:
     *
     * - Sem ID: createProductData(productData)
     * - Com ID: updateProductData(productId, productData)
     */
    async function submitProductForm(event) {
        event.preventDefault();

        ui.clearFormMessage(formMessage);

        // Executa as validações nativas do HTML.
        if (!productForm.checkValidity()) {
            productForm.reportValidity();
            return;
        }

        // Remove espaços nas extremidades antes de validar e enviar.
        const name = nameInput.value.trim();
        const description = descriptionInput.value.trim();
        const brandName = brandNameInput.value.trim();

        /*
            required aceita uma sequência de espaços como valor preenchido.
            Por isso validamos novamente depois de aplicar trim().
        */
        if (!name || !description || !brandName) {
            ui.showFormMessage(
                formMessage,
                "Nome, descrição e marca não podem conter apenas espaços.",
                "error"
            );
            return;
        }

        /*
            O payload segue tanto o CreateProductDTO quanto o UpdateProductDTO.

            Não enviamos:
            - sku: o backend gera automaticamente na criação.
            - id: o ID fica somente na URL do PATCH.
            - stockQuantity: estoque não existe mais.
            - details: o DTO atual é plano.
        */
        const productData = {
            name: name,
            description: description,
            price: Number(priceInput.value),
            category: Number(categoryInput.value),
            brandName: brandName,
            weightOrVolume: Number(weightOrVolumeInput.value),
            measureType: Number(measureTypeInput.value)
        };

        /*
            O campo oculto fica vazio no modo de criação.
            Durante a edição, ele contém o Guid do produto.
        */
        const productId = productIdInput.value.trim();
        const isEditing = Boolean(productId);

        const loadingMessage = isEditing
            ? "Atualizando produto..."
            : "Criando produto...";

        const successMessage = isEditing
            ? "Produto atualizado com sucesso."
            : "Produto criado com sucesso.";

        try {
            ui.setSubmitButtonState(
                submitButton,
                true,
                loadingMessage,
                isEditing
            );

            if (isEditing) {
                await api.updateProduct(productId, productData);
            } else {
                await api.createProduct(productData);
            }

            /*
                Depois de criar ou editar, limpamos o formulário e retornamos
                ao modo de criação.
            */
            resetProductForm();

            ui.showFormMessage(formMessage, successMessage, "success");

            await reloadProducts();
        } catch (error) {
            console.error(error);

            ui.showFormMessage(formMessage, error.message, "error");
        } finally {
            ui.setSubmitButtonState(
                submitButton,
                false,
                "",
                Boolean(productIdInput.value)
            );
        }
    }


    /**
     * Preenche o formulário com os dados de um produto existente.
     *
     * O produto vem do retorno atual de GET /api/Product:
     *
     * - Os dados principais estão diretamente em product.
     * - Marca, peso/volume e medida estão em product.details.
     * - O SKU não é colocado no formulário porque é imutável.
     *
     * @param {object} product Produto selecionado para edição.
     */
    function editProduct(product) {
        /*
            Guardamos o ID somente no input hidden.
            Ele será usado na URL do PATCH.
        */
        productIdInput.value = product.id;

        // Preenche os campos principais retornados pelo backend.
        nameInput.value = product.name ?? "";
        descriptionInput.value = product.description ?? "";
        priceInput.value = product.price ?? "";
        categoryInput.value = String(product.category ?? "");

        /*
            O retorno GET organiza os dados complementares dentro de details.
        */
        brandNameInput.value = product.details?.brandName ?? "";
        weightOrVolumeInput.value =
            product.details?.weightOrVolume ?? "";
        measureTypeInput.value =
            String(product.details?.measureType ?? "");

        // Atualiza visualmente o formulário para o modo de edição.
        formTitle.textContent = "Editar Produto";
        productForm.classList.add("edit-mode");
        submitButton.textContent = "Salvar";
        cancelButton.style.display = "inline-block";

        /*
            Durante a edição, limpamos a área de produtos semelhantes
            para não confundir a edição com um novo cadastro.
        */
        clearSimilarProducts();
        ui.clearFormMessage(formMessage);

        /*
            É importante chamar esta função depois de definir measureType.
            Assim, Un volta a bloquear o valor e define 1 automaticamente.
        */
        updateMeasurementField();

        // Facilita o início da alteração pelo usuário.
        nameInput.focus();
    }


    /**
     * Cancela a edição atual.
     *
     * Nenhuma requisição é enviada ao backend.
     */
    function cancelEdit() {
        resetProductForm();
        ui.clearFormMessage(formMessage);
    }


    /**
     * Retorna o formulário ao estado inicial de criação.
     */
    function resetProductForm() {
        // Limpa todos os campos visíveis e o campo hidden.
        productForm.reset();
        productForm.classList.remove("edit-mode");
        productIdInput.value = "";

        // Restaura os textos e botões do modo de criação.
        formTitle.textContent = "Novo Produto";
        submitButton.textContent = "Criar Produto";
        cancelButton.style.display = "none";

        // Limpa resultados antigos e ajusta novamente o campo de medida.
        clearSimilarProducts();
        updateMeasurementField();
    }
}