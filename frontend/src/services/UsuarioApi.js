import { HTTPClient } from './client';

const UsuarioApi = {
    async ListarAsync() {
        try {
            const response = await HTTPClient.get('/usuario');
            return response.data;
        } catch (error) {
            console.error('Erro ao listar usuários:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/usuario/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar usuário:', error);
            throw error;
        }
    },

    // data = { Nome, Email, Telefone, Senha }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/usuario', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar usuário:', error);
            throw error;
        }
    },

    // data = { Nome, Email, Telefone, Senha }
    async AtualizarAsync(id, data) {
        try {
            const response = await HTTPClient.put(`/usuario/${id}`, data);
            return response.data;
        } catch (error) {
            console.error('Erro ao atualizar usuário:', error);
            throw error;
        }
    },

    // data = { SenhaAtual, NovaSenha }
    async AlterarSenhaAsync(id, data) {
        try {
            const response = await HTTPClient.put(`/usuario/${id}/senha`, data);
            return response.data;
        } catch (error) {
            console.error('Erro ao alterar senha:', error);
            throw error;
        }
    },

    async DeletarAsync(id) {
        try {
            const response = await HTTPClient.delete(`/usuario/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao deletar usuário:', error);
            throw error;
        }
    },
};

export default UsuarioApi;
