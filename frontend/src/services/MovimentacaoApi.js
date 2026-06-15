import { HTTPClient } from './client';

const MovimentacaoApi = {
    // params = { page, pageSize, produtoId, tipo, de, ate }
    async ListarAsync(params) {
        try {
            const response = await HTTPClient.get('/movimentacao', { params });
            return response.data;
        } catch (error) {
            console.error('Erro ao listar movimentações:', error);
            throw error;
        }
    },

    // data = { ProdutoID, UsuarioID, TipoMovimentacao, MotivoMovimentacao, Quantidade }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/movimentacao', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar movimentação:', error);
            throw error;
        }
    },
};

export default MovimentacaoApi;
