import { HTTPClient } from './client';

const ProdutoApi = {
    // params = { page, pageSize, nome, categoriaId }
    async ListarAsync(params) {
        try {
            const response = await HTTPClient.get('/produto', { params });
            return response.data;
        } catch (error) {
            console.error('Erro ao listar produtos:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/produto/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar produto:', error);
            throw error;
        }
    },

    // data = { Nome, Descricao, Preco, EstoqueAtual, EstoqueMinimo, CategoriaID }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/produto', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar produto:', error);
            throw error;
        }
    },

    // data = { Nome, Descricao, Preco, EstoqueAtual, EstoqueMinimo, CategoriaID }
    async AtualizarAsync(id, data) {
        try {
            const response = await HTTPClient.put(`/produto/${id}`, data);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar produto:', error);
            throw error;
        }
    },

    async DeletarAsync(id) {
        try {
            const response = await HTTPClient.delete(`/produto/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao deletar produto:', error);
            throw error;
        }
    },
};

export default ProdutoApi;
