import { HTTPClient } from './client';

const AIApi = {
    // VIEW: vw_ResumoVendas
    async AnalisarVendasAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-vendas');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar vendas com IA:', error);
            throw error;
        }
    },

    // VIEW: vw_ProdutosAbaixoMinimo
    async AnalisarEstoqueAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-estoque');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar estoque com IA:', error);
            throw error;
        }
    },

    // VIEW: vw_VendasPorFormaPagamento
    async AnalisarFormasPagamentoAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-formas-pagamento');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar formas de pagamento com IA:', error);
            throw error;
        }
    },

    // SP: sp_ProdutosMaisVendidos
    async AnalisarProdutosMaisVendidosAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-produtos-mais-vendidos');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar produtos mais vendidos com IA:', error);
            throw error;
        }
    },

    // FUNCTION: fn_TotalVendasCliente
    async AnalisarClienteAsync(clienteId) {
        try {
            const response = await HTTPClient.post(`/ai/analisar-cliente/${clienteId}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar cliente com IA:', error);
            throw error;
        }
    },
};

export default AIApi;
