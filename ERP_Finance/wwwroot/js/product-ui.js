/*
    Este arquivo concentra funções relacionadas à interface visual.

    Ele não chama a API e não decide quando criar, editar ou excluir.
    Sua responsabilidade é transformar dados em elementos visuais
    e atualizar mensagens e controles da tela.
*/


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
 *
 * @param {Array} products Produtos retornados pela API.
 * @param {HTMLElement} productsList Elemento que receberá os cards.
 * @param {Function} onEdit Função executada ao clicar em Editar.
 * @param {Function} onDelete Função executada ao clicar em Excluir.
 */
export function renderProducts(
    products,
    productsList,
    onEdit,
    onDelete
) {
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

            const weightOrVolume =
                product.details?.weightOrVolume ?? "Não informado";

            const measureType =
                getMeasureTypeName(product.details?.measureType);

            const measurementLabel =
                getMeasurementLabel(product.details?.measureType);

            const formattedPrice = formatCurrency(product.price);
            const categoryName = getCategoryName(product.category);

            return `
                <article class="product-card">
                    <div class="product-card-header">
                        <div>
                            <h3>${escapeHtml(product.name)}</h3>

                            <span class="product-sku">
                                SKU: ${escapeHtml(product.sku)}
                            </span>
                        </div>

                        <strong class="product-price">
                            ${formattedPrice}
                        </strong>
                    </div>

                    <p class="product-description">
                        ${escapeHtml(
                product.description || "Sem descrição cadastrada."
            )}
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
                                <dd>
                                    ${escapeHtml(String(weightOrVolume))}
                                    ${measureType}
                                </dd>
                            </div>
                        </dl>

                        <div class="product-card-actions">
                            <!--
                                O botão Editar não chama a API diretamente.
                                Ele entrega o produto selecionado ao módulo
                                responsável pelo formulário.
                            -->
                            <button
                                type="button"
                                class="edit-button"
                                data-product-id="${escapeHtml(product.id)}">
                                Editar
                            </button>

                            <!--
                                O botão Excluir também não exclui imediatamente.
                                Ele abre o modal de confirmação no app.js.
                            -->
                            <button
                                type="button"
                                class="delete-button"
                                data-product-id="${encodeURIComponent(product.id)}"
                                data-product-name="${escapeHtml(product.name)}">
                                Excluir
                            </button>
                        </div>
                    </div>
                </article>
            `;
        })
        .join("");

    /*
        Como os cards foram criados com innerHTML,
        os eventos precisam ser registrados depois da renderização.
    */
    const editButtons = productsList.querySelectorAll(".edit-button");

    editButtons.forEach((button, index) => {
        button.addEventListener("click", () => {
            /*
                O índice do botão corresponde ao índice do produto
                no array utilizado para montar os cards.
            */
            onEdit(products[index]);
        });
    });

    const deleteButtons = productsList.querySelectorAll(".delete-button");

    deleteButtons.forEach((button) => {
        button.addEventListener("click", () => {
            onDelete(
                button.dataset.productId,
                button.dataset.productName
            );
        });
    });
}


/**
 * Converte o número de ProductCategory para texto.
 *
 * Os números correspondem ao enum ProductCategory do backend.
 *
 * @param {number} category Valor numérico da categoria.
 * @returns {string} Nome da categoria.
 */
export function getCategoryName(category) {
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
export function getMeasurementLabel(measureType) {
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
 *
 * @param {number} measureType Valor numérico da medida.
 * @returns {string} Nome da medida.
 */
export function getMeasureTypeName(measureType) {
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
 *
 * @param {number} value Preço recebido da API.
 * @returns {string} Preço formatado.
 */
export function formatCurrency(value) {
    return new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    }).format(value);
}


/**
 * Escapa texto antes de inseri-lo por innerHTML.
 *
 * Isso evita que valores vindos da API sejam interpretados
 * como HTML dentro dos cards.
 *
 * @param {*} value Valor que será transformado em texto seguro.
 * @returns {string} Texto escapado.
 */
export function escapeHtml(value) {
    const temporaryElement = document.createElement("div");

    temporaryElement.textContent = value ?? "";

    return temporaryElement.innerHTML;
}


/**
 * Mostra uma mensagem abaixo do formulário.
 *
 * @param {HTMLElement} formMessage Elemento da mensagem.
 * @param {string} message Texto exibido.
 * @param {string} type Classe usada para definir o estilo da mensagem.
 */
export function showFormMessage(formMessage, message, type) {
    formMessage.textContent = message;
    formMessage.className = `message ${type}-message`;
}


/**
 * Limpa uma mensagem anterior do formulário.
 *
 * @param {HTMLElement} formMessage Elemento da mensagem.
 */
export function clearFormMessage(formMessage) {
    formMessage.textContent = "";
    formMessage.className = "message";
}


/**
 * Atualiza o estado do botão principal do formulário.
 *
 * Esta função não decide se a operação é criação ou edição.
 * Ela recebe o texto que deve aparecer durante o carregamento.
 *
 * @param {HTMLElement} submitButton Botão principal.
 * @param {boolean} isLoading Indica se existe uma requisição em andamento.
 * @param {string} loadingText Texto exibido durante a requisição.
 * @param {boolean} isEditing Indica se o formulário está editando.
 */
export function setSubmitButtonState(
    submitButton,
    isLoading,
    loadingText,
    isEditing
) {
    submitButton.disabled = isLoading;

    if (isLoading) {
        submitButton.textContent = loadingText;
        return;
    }

    submitButton.textContent = isEditing
        ? "Salvar"
        : "Criar Produto";
}


/**
 * Altera o estado do botão Excluir dentro do modal.
 *
 * @param {HTMLElement} confirmDeleteButton Botão de confirmação.
 * @param {boolean} isLoading Indica se a exclusão está em andamento.
 */
export function setDeleteButtonState(
    confirmDeleteButton,
    isLoading
) {
    confirmDeleteButton.disabled = isLoading;

    confirmDeleteButton.textContent = isLoading
        ? "Excluindo..."
        : "Excluir";
}