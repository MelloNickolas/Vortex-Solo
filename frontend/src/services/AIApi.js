import { HTTPClient } from './client';

const AIApi = {
    async AnalisarVendasAsync() {
        const response = await HTTPClient.post('/ai/analisar-vendas');
        return response.data;
    },

    async AnalisarEstoqueAsync() {
        const response = await HTTPClient.post('/ai/analisar-estoque');
        return response.data;
    },
};

export default AIApi;
