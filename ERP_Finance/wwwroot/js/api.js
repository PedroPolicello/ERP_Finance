/*
    URL base dos endpoints de produto.

    Este arquivo concentra somente a comunicação com a API.
    A interface e as regras visuais continuam nos outros módulos.
*/
const productApiUrl = "/api/Product";


/**
 * Busca todos os produtos.
 *
 * Endpoint:
 * GET /api/Product
 */
export async function getProducts() {
    const response = await fetch(productApiUrl);

    if (!response.ok) {
        throw new Error("Não foi possível carregar os produtos.");
    }

    return await response.json();
}


/**
 * Busca produtos com nomes semelhantes.
 *
 * Endpoint:
 * GET /api/Product/search?name=...
 *
 * @param {string} name Nome usado na pesquisa.
 */
export async function searchProductsByName(name) {
    const response = await fetch(
        `${productApiUrl}/search?name=${encodeURIComponent(name)}`
    );

    if (!response.ok) {
        throw new Error("Não foi possível buscar produtos semelhantes.");
    }

    return await response.json();
}


/**
 * Cria um produto.
 *
 * Endpoint:
 * POST /api/Product
 *
 * @param {object} productData Dados planos do CreateProductDTO.
 */
export async function createProduct(productData) {
    const response = await fetch(productApiUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(productData)
    });

    if (!response.ok) {
        throw new Error(await getApiErrorMessage(response));
    }
}


/**
 * Atualiza parcialmente um produto.
 *
 * Endpoint:
 * PATCH /api/Product/{id}
 *
 * @param {string} productId ID do produto.
 * @param {object} productData Dados planos do UpdateProductDTO.
 */
export async function updateProduct(productId, productData) {
    const response = await fetch(
        `${productApiUrl}/${encodeURIComponent(productId)}`,
        {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(productData)
        }
    );

    if (!response.ok) {
        throw new Error(await getApiErrorMessage(response));
    }
}


/**
 * Exclui um produto.
 *
 * Endpoint:
 * DELETE /api/Product/{id}
 *
 * @param {string} productId ID do produto.
 */
export async function deleteProductById(productId) {
    const response = await fetch(
        `${productApiUrl}/${productId}`,
        {
            method: "DELETE"
        }
    );

    if (!response.ok) {
        throw new Error(await getApiErrorMessage(response));
    }
}


/**
 * Lê a resposta de erro do backend e tenta extrair a mensagem mais útil.
 *
 * detail é preferido porque normalmente contém a explicação específica.
 * title é somente uma alternativa: pode ser algo genérico como "Bad Request".
 *
 * Esta função fica aqui porque os erros são recebidos durante requisições HTTP.
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
