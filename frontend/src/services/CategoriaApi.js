import { HTTPClient } from './client';

const CategoriaApi = {
    async ListarAsync() {
        try {
            const response = await HTTPClient.get('/categoria');
            return response.data;
        } catch (error) {
            console.error('Erro ao listar categorias:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/categoria/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar categoria:', error);
            throw error;
        }
    },

    // data = { Nome }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/categoria', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar categoria:', error);
            throw error;
        }
    },

    // data = { Nome }
    async AtualizarAsync(id, data) {
        try {
            const response = await HTTPClient.put(`/categoria/${id}`, data);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar categoria:', error);
            throw error;
        }
    },

    async DeletarAsync(id) {
        try {
            const response = await HTTPClient.delete(`/categoria/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao deletar categoria:', error);
            throw error;
        }
    },
};

export default CategoriaApi;
