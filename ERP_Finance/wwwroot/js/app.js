// URL base dos endpoints de produto.
const productApiUrl = "/api/Product";

// Elementos principais do formulário e da listagem.
const productForm = document.getElementById("product-form");
const productsList = document.getElementById("products-list");
const refreshButton = document.getElementById("refresh-btn");
const formMessage = document.getElementById("form-message");

// Campos atuais do formulário.
// SKU e quantidade de estoque não existem mais no frontend.
const nameInput = document.getElementById("name");
// Área abaixo do campo Nome onde os produtos semelhantes serão exibidos.
const similarProductsContainer = document.getElementById("similar-products");
const descriptionInput = document.getElementById("description");
const priceInput = document.getElementById("price");
const categoryInput = document.getElementById("category");
const brandNameInput = document.getElementById("brandName");
const weightOrVolumeInput = document.getElementById("weightOrVolume");

// Rótulo que mudará entre Peso, Volume e Unidade.
const weightOrVolumeLabel = document.getElementById(
    "weight-or-volume-label"
);

const measureTypeInput = document.getElementById("measureType");

// Elementos do modal personalizado de confirmação de exclusão.
const deleteConfirmationDialog = document.getElementById(
    "delete-confirmation-dialog"
);

const deleteConfirmationMessage = document.getElementById(
    "delete-confirmation-message"
);

const cancelDeleteButton = document.getElementById("cancel-delete-button");
const confirmDeleteButton = document.getElementById("confirm-delete-button");

/*
    Guarda o produto escolhido para exclusão enquanto o modal estiver aberto.
    Ele começa vazio porque ninguém foi selecionado ainda.
*/
let productPendingDeletion = null;

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

// Carrega a lista quando a página termina de abrir.
document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
    updateMeasurementField();
});
// Atualiza a lista manualmente.
refreshButton.addEventListener("click", loadProducts);

// Envia o formulário para criar um produto.
productForm.addEventListener("submit", createProduct);

// Busca produtos com nomes semelhantes enquanto a pessoa digita.
nameInput.addEventListener("input", handleSimilarProductsSearch);

// Ajusta o campo de valor conforme a medida selecionada.
measureTypeInput.addEventListener("change", updateMeasurementField);

// Fecha o modal sem excluir.
cancelDeleteButton.addEventListener("click", closeDeleteConfirmationDialog);

// Executa a exclusão depois da confirmação no modal.
confirmDeleteButton.addEventListener("click", confirmDeleteProductDeletion);


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
            O campo fica bloqueado para evitar valores como 2,5 Unidades.
        */
        weightOrVolumeLabel.textContent = "Unidade";
        weightOrVolumeInput.value = 1;
        weightOrVolumeInput.disabled = true;

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
 * Consulta o endpoint de busca por nomes semelhantes.
 *
 * Endpoint:
 * GET /api/Product/search?name=...
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
        const response = await fetch(
            `${productApiUrl}/search?name=${encodeURIComponent(name)}`
        );

        if (!response.ok) {
            throw new Error("Não foi possível buscar produtos semelhantes.");
        }

        const products = await response.json();

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
            Não exibimos erro visual aqui para não assustar a pessoa enquanto digita.
            A prevenção de duplicidade é uma ajuda, não deve bloquear o cadastro.
        */
        clearSimilarProducts();
    }
}

/**
 * Desenha a lista de produtos semelhantes abaixo do campo Nome.
 *
 * @param {Array} products Produtos retornados por GET /api/Product/search.
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
                            <strong>${escapeHtml(product.name)}</strong>
                            <span>SKU: ${escapeHtml(product.sku)}</span>
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
 * Busca todos os produtos.
 *
 * Endpoint:
 * GET /api/Product
 */
async function loadProducts() {
    try {
        productsList.innerHTML = "<p>Carregando produtos...</p>";

        const response = await fetch(productApiUrl);

        if (!response.ok) {
            throw new Error("Não foi possível carregar os produtos.");
        }

        const products = await response.json();

        renderProducts(products);
    } catch (error) {
        console.error(error);

        productsList.innerHTML =
            "<p class=\"error-message\">Erro ao carregar os produtos. Tente novamente.</p>";
    }
}

/**
 * Cria um produto.
 *
 * Endpoint:
 * POST /api/Product
 */
async function createProduct(event) {
    event.preventDefault();

    clearFormMessage();

    // Executa as validações nativas: required, maxlength, min e select obrigatório.
    if (!productForm.checkValidity()) {
        productForm.reportValidity();
        return;
    }

    // Remove espaços nas extremidades antes de validar e enviar ao backend.
    const name = nameInput.value.trim();
    const description = descriptionInput.value.trim();
    const brandName = brandNameInput.value.trim();

    /*
        required aceita uma sequência de espaços como valor preenchido.
        Por isso validamos novamente depois de aplicar trim().
    */
    if (!name || !description || !brandName) {
        showFormMessage(
            "Nome, descrição e marca não podem conter apenas espaços.",
            "error"
        );
        return;
    }

    /*
        O payload segue o CreateProductDTO atual.

        Não enviamos:
        - sku: o backend gera automaticamente.
        - stockQuantity: estoque não existe mais.
        - details: o DTO de criação é plano.
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

    try {
        setSubmitButtonState(true, "Criando produto...");

        const response = await fetch(productApiUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(productData)
        });

        if (!response.ok) {
            const errorMessage = await getApiErrorMessage(response);

            throw new Error(errorMessage);
        }

        // O backend gera SKU, ID e datas; a lista é recarregada para mostrar o retorno.
        productForm.reset();

        showFormMessage("Produto criado com sucesso.", "success");

        await loadProducts();
    } catch (error) {
        console.error(error);

        showFormMessage(error.message, "error");
    } finally {
        setSubmitButtonState(false, "Criar Produto");
    }
}

/**
 * Abre o modal e guarda qual produto poderá ser excluído.
 *
 * @param {string} productId Identificador do produto.
 * @param {string} productName Nome usado na mensagem de confirmação.
 */
function deleteProduct(productId, productName) {
    productPendingDeletion = {
        id: productId,
        name: productName
    };

    deleteConfirmationMessage.textContent =
        `Deseja realmente excluir o produto "${productName}"?`;

    deleteConfirmationDialog.showModal();
}

/**
 * Fecha o modal e limpa o produto pendente.
 */
function closeDeleteConfirmationDialog() {
    deleteConfirmationDialog.close();

    productPendingDeletion = null;
}

/**
 * Executa DELETE após a confirmação.
 *
 * Endpoint:
 * DELETE /api/Product/{id}
 */
async function confirmDeleteProductDeletion() {
    if (!productPendingDeletion) {
        return;
    }

    try {
        setDeleteButtonState(true);

        const response = await fetch(
            `${productApiUrl}/${productPendingDeletion.id}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {
            const errorMessage = await getApiErrorMessage(response);

            throw new Error(errorMessage);
        }

        closeDeleteConfirmationDialog();

        showFormMessage("Produto excluído com sucesso.", "success");

        await loadProducts();
    } catch (error) {
        console.error(error);

        showFormMessage(error.message, "error");
    } finally {
        setDeleteButtonState(false);
    }
}

/**
 * Ativa ou desativa o botão principal do formulário.
 *
 * @param {boolean} isLoading Indica se existe uma requisição em andamento.
 * @param {string} loadingText Texto apresentado enquanto a API processa a ação.
 */
function setSubmitButtonState(isLoading, loadingText) {
    const submitButton = document.getElementById("submit-btn");

    submitButton.disabled = isLoading;
    submitButton.textContent = isLoading
        ? loadingText
        : "Criar Produto";
}

/**
 * Altera o botão Excluir que fica dentro do modal.
 */
function setDeleteButtonState(isLoading) {
    confirmDeleteButton.disabled = isLoading;
    confirmDeleteButton.textContent = isLoading
        ? "Excluindo..."
        : "Excluir";
}

/**
 * Mostra uma mensagem abaixo do formulário.
 */
function showFormMessage(message, type) {
    formMessage.textContent = message;
    formMessage.className = `message ${type}-message`;
}

/**
 * Limpa uma mensagem anterior do formulário.
 */
function clearFormMessage() {
    formMessage.textContent = "";
    formMessage.className = "message";
}

/**
 * Lê a resposta de erro do backend e tenta extrair a mensagem mais útil.
 *
 * detail é preferido porque normalmente contém a explicação específica.
 * title é somente uma alternativa: pode ser algo genérico como "Bad Request".
 */
async function getApiErrorMessage(response) {
    const responseText = await response.text();

    if (!responseText) {
        return `Não foi possível concluir a operação. Código HTTP: ${response.status}.`;
    }

    try {
        const errorData = JSON.parse(responseText);

        if (errorData.detail) {
            return errorData.detail;
        }

        if (errorData.message) {
            return errorData.message;
        }

        if (errorData.title) {
            return errorData.title;
        }

        return responseText;
    } catch {
        return responseText;
    }
}

/**
 * Desenha os cards usando o JSON atual retornado por GET /api/Product.
 *
 * Campos usados:
 * product.id
 * product.sku
 * product.name
 * product.description
 * product.price
 * product.category
 * product.details.brandName
 * product.details.weightOrVolume
 * product.details.measureType
 */
function renderProducts(products) {
    if (!Array.isArray(products) || products.length === 0) {
        productsList.innerHTML = "<p>Nenhum produto cadastrado.</p>";
        return;
    }

    productsList.innerHTML = products
        .map((product) => {
            /*
                ?. evita que a tela quebre se details for null/undefined.
                ?? define um valor padrão somente quando o valor for null/undefined.
                Assim, por exemplo, o número 0 não seria trocado indevidamente.
            */
            const brandName = product.details?.brandName ?? "Não informado";
            const weightOrVolume = product.details?.weightOrVolume ?? "Não informado";
            const measureType = getMeasureTypeName(product.details?.measureType);
            const measurementLabel = getMeasurementLabel(product.details?.measureType);

            const formattedPrice = formatCurrency(product.price);
            const categoryName = getCategoryName(product.category);
            const productId = encodeURIComponent(product.id);

            return `
                <article class="product-card">
                    <div class="product-card-header">
                        <div>
                            <h3>${escapeHtml(product.name)}</h3>
                            <span class="product-sku">SKU: ${escapeHtml(product.sku)}</span>
                        </div>

                        <strong class="product-price">${formattedPrice}</strong>
                    </div>

                    <p class="product-description">
                        ${escapeHtml(product.description || "Sem descrição cadastrada.")}
                    </p>

                    <div class="product-card-details">
                        <dl class="product-info">
                            <div>
                                <dt>Categoria</dt>
                                <dd>${categoryName}</dd>
                            </div>

                            <div>
                                <dt>Marca</dt>
                                <dd>${escapeHtml(brandName)}</dd>
                            </div>

                            <div>
                                <dt>${measurementLabel}</dt>
                                <dd>${escapeHtml(String(weightOrVolume))} ${measureType}</dd>
                            </div>
                        </dl>

                        <div class="product-card-actions">
                            <button
                                type="button"
                                class="delete-button"
                                data-product-id="${productId}"
                                data-product-name="${escapeHtml(product.name)}">
                                Excluir
                            </button>
                        </div>
                    </div>
                </article>
            `;
        })
        .join("");

    const deleteButtons = document.querySelectorAll(".delete-button");

    deleteButtons.forEach((button) => {
        button.addEventListener("click", () => {
            deleteProduct(
                button.dataset.productId,
                button.dataset.productName
            );
        });
    });
}

/**
 * Converte o número de ProductCategory para texto.
 */
function getCategoryName(category) {
    const categories = {
        0: "Salgados",
        1: "Assados",
        2: "Salgadinhos",
        3: "Doces",
        4: "Bebidas"
    };

    return categories[category] ?? "Não informada";
}

/**
 * Define o título mostrado no card conforme o MeasureType.
 *
 * 0 = Unit       -> Unidade
 * 1 = Kilogram   -> Peso
 * 2 = Gram       -> Peso
 * 3 = Liter      -> Volume
 * 4 = Milliliter -> Volume
 *
 * @param {number} measureType Valor numérico do enum MeasureType.
 * @returns {string} Título adequado para o dado de medida.
 */
function getMeasurementLabel(measureType) {
    const measurementLabels = {
        0: "Unidade",
        1: "Peso",
        2: "Peso",
        3: "Volume",
        4: "Volume"
    };

    return measurementLabels[measureType] ?? "Medida";
}

/**
 * Converte o número de MeasureType para texto.
 */
function getMeasureTypeName(measureType) {
    const measureTypes = {
        0: "Unidade",
        1: "Quilograma",
        2: "Grama",
        3: "Litro",
        4: "Mililitro"
    };

    return measureTypes[measureType] ?? "";
}

/**
 * Formata um preço para Real brasileiro.
 */
function formatCurrency(value) {
    return new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    }).format(value);
}

/**
 * Escapa texto antes de inseri-lo por innerHTML.
 */
function escapeHtml(value) {
    const temporaryElement = document.createElement("div");
    temporaryElement.textContent = value ?? "";

    return temporaryElement.innerHTML;
}