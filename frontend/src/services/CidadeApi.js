import { HTTPClient } from './client';

const CidadeApi = {
    async ListarPorEstadoAsync(estadoId) {
        try {
            const response = await HTTPClient.get(`/cidade/estado/${estadoId}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao listar cidades:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/cidade/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar cidade:', error);
            throw error;
        }
    },
};

export default CidadeApi;
