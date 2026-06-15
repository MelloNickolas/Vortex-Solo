import { HTTPClient } from './client';

const EstadoApi = {
    async ListarAsync() {
        try {
            const response = await HTTPClient.get('/estado');
            return response.data;
        } catch (error) {
            console.error('Erro ao listar estados:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/estado/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar estado:', error);
            throw error;
        }
    },
};

export default EstadoApi;
