import { HTTPClient } from './client';

const RelatorioApi = {
    async BuscarResumoAsync() {
        try {
            const response = await HTTPClient.get('/relatorio/resumo');
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar resumo:', error);
            throw error;
        }
    },

    async ListarProdutosAbaixoMinimoAsync() {
        try {
            const response = await HTTPClient.get('/relatorio/produtos-abaixo-minimo');
            return response.data;
        } catch (error) {
            console.error('Erro ao listar produtos abaixo do mínimo:', error);
            throw error;
        }
    },

    async ListarFormasPagamentoAsync() {
        try {
            const response = await HTTPClient.get('/relatorio/formas-pagamento');
            return response.data;
        } catch (error) {
            console.error('Erro ao listar formas de pagamento:', error);
            throw error;
        }
    },

    async ListarProdutosMaisVendidosAsync(top = 10) {
        try {
            const response = await HTTPClient.get('/relatorio/produtos-mais-vendidos', { params: { top } });
            return response.data;
        } catch (error) {
            console.error('Erro ao listar produtos mais vendidos:', error);
            throw error;
        }
    },

    // fn_TotalVendasCliente
    async ConsultarTotalClienteAsync(clienteId) {
        try {
            const response = await HTTPClient.get(`/relatorio/total-cliente/${clienteId}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao consultar total do cliente:', error);
            throw error;
        }
    },
};

export default RelatorioApi;
