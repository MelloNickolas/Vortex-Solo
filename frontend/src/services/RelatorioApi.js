import { HTTPClient } from './client';

const RelatorioApi = {
    async BuscarResumoAsync() {
        const response = await HTTPClient.get('/relatorio/resumo');
        return response.data;
    },

    async ListarProdutosAbaixoMinimoAsync() {
        const response = await HTTPClient.get('/relatorio/produtos-abaixo-minimo');
        return response.data;
    },

    async ListarFormasPagamentoAsync() {
        const response = await HTTPClient.get('/relatorio/formas-pagamento');
        return response.data;
    },

    async ListarProdutosMaisVendidosAsync(top = 10) {
        const response = await HTTPClient.get('/relatorio/produtos-mais-vendidos', { params: { top } });
        return response.data;
    },
};

export default RelatorioApi;
