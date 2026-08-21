/*
    Este é o ponto de entrada do frontend.

    O app.js coordena os módulos, mas não concentra mais todas as regras.

    Responsabilidades principais:
    - localizar os elementos HTML;
    - importar os módulos da API, da interface e do formulário;
    - carregar os produtos;
    - controlar o modal de exclusão;
    - conectar os eventos dos cards.
*/

import * as api from "./api.js";

import * as ui from "./product-ui.js";

import {
    createProductFormController
} from "./product-form.js";


// Elementos principais da página.
const productForm = document.getElementById("product-form");
const productsList = document.getElementById("products-list");
const refreshButton = document.getElementById("refresh-btn");
const formMessage = document.getElementById("form-message");

// Elementos usados pelo formulário de criação e edição.
const productIdInput = document.getElementById("product-id");
const formTitle = document.getElementById("form-title");
const submitButton = document.getElementById("submit-btn");
const cancelButton = document.getElementById("cancel-btn");
const nameInput = document.getElementById("name");
const similarProductsContainer = document.getElementById("similar-products");
const descriptionInput = document.getElementById("description");
const priceInput = document.getElementById("price");
const categoryInput = document.getElementById("category");
const brandNameInput = document.getElementById("brandName");
const weightOrVolumeInput = document.getElementById("weightOrVolume");
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
    Agrupa os elementos usados pelo módulo de formulário.

    O app.js busca os elementos do HTML e os entrega ao product-form.js.
    Assim, o módulo de formulário não precisa consultar o DOM da página inteira.
*/
const formElements = {
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
};


/*
    Cria o controlador do formulário.

    O objeto retornado contém funções que os cards precisarão chamar,
    principalmente editProduct.
*/
const productFormController = createProductFormController({
    elements: formElements,
    api,
    ui,
    reloadProducts: loadProducts
});


// Atualiza a lista manualmente.
refreshButton.addEventListener("click", loadProducts);

// Fecha o modal sem excluir.
cancelDeleteButton.addEventListener("click", closeDeleteConfirmationDialog);

// Executa a exclusão depois da confirmação no modal.
confirmDeleteButton.addEventListener(
    "click",
    confirmDeleteProductDeletion
);

// Carrega a lista quando o módulo termina de ser executado.
loadProducts();


/**
 * Busca todos os produtos e atualiza a lista visual.
 *
 * A chamada HTTP agora está no api.js.
 * A criação dos cards agora está no product-ui.js.
 */
async function loadProducts() {
    try {
        productsList.innerHTML = "<p>Carregando produtos...</p>";

        const products = await api.getProducts();

        ui.renderProducts(
            products,
            productsList,
            productFormController.editProduct,
            deleteProduct
        );
    } catch (error) {
        console.error(error);

        productsList.innerHTML =
            "<p class=\"error-message\">Erro ao carregar os produtos. Tente novamente.</p>";
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
 * Endpoint utilizado pelo api.js:
 * DELETE /api/Product/{id}
 */
async function confirmDeleteProductDeletion() {
    if (!productPendingDeletion) {
        return;
    }

    try {
        ui.setDeleteButtonState(confirmDeleteButton, true);

        await api.deleteProductById(productPendingDeletion.id);

        closeDeleteConfirmationDialog();

        ui.showFormMessage(
            formMessage,
            "Produto excluído com sucesso.",
            "success"
        );

        await loadProducts();
    } catch (error) {
        console.error(error);

        ui.showFormMessage(formMessage, error.message, "error");
    } finally {
        ui.setDeleteButtonState(confirmDeleteButton, false);
    }
}