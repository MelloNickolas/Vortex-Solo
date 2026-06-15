import { HTTPClient } from './client';

const ClienteApi = {
    // params = { page, pageSize, busca, ativo }
    async ListarAsync(params) {
        try {
            const response = await HTTPClient.get('/cliente', { params });
            return response.data;
        } catch (error) {
            console.error('Erro ao listar clientes:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/cliente/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar cliente:', error);
            throw error;
        }
    },

    // data = { Nome, CPF, Telefone, Email, Rua, Numero, CidadeID }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/cliente', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar cliente:', error);
            throw error;
        }
    },

    // data = { Nome, CPF, Telefone, Email, Rua, Numero, CidadeID }
    async AtualizarAsync(id, data) {
        try {
            const response = await HTTPClient.put(`/cliente/${id}`, data);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar cliente:', error);
            throw error;
        }
    },

    // Soft delete — marca como inativo, não remove do banco
    async DeletarAsync(id) {
        try {
            const response = await HTTPClient.delete(`/cliente/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao deletar cliente:', error);
            throw error;
        }
    },

    // Reativa um cliente que foi desativado
    async ReativarAsync(id) {
        try {
            const response = await HTTPClient.patch(`/cliente/${id}/reativar`);
            return response.data;
        } catch (error) {
            console.error('Erro ao reativar cliente:', error);
            throw error;
        }
    },
};

export default ClienteApi;
