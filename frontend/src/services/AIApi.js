import { HTTPClient } from './client';

const AIApi = {
    async AnalisarVendasAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-vendas');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar vendas com IA:', error);
            throw error;
        }
    },

    async AnalisarEstoqueAsync() {
        try {
            const response = await HTTPClient.post('/ai/analisar-estoque');
            return response.data;
        } catch (error) {
            console.error('Erro ao analisar estoque com IA:', error);
            throw error;
        }
    },
};

export default AIApi;
