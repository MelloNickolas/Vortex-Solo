import { HTTPClient } from './client';

const VendaApi = {
    // params = { page, pageSize, status, de, ate }
    async ListarAsync(params) {
        try {
            const response = await HTTPClient.get('/venda', { params });
            return response.data;
        } catch (error) {
            console.error('Erro ao listar vendas:', error);
            throw error;
        }
    },

    async BuscarPorIdAsync(id) {
        try {
            const response = await HTTPClient.get(`/venda/${id}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar venda:', error);
            throw error;
        }
    },

    // data = { ClienteID, UsuarioID, FormaPagamento, Itens: [{ ProdutoID, Quantidade }] }
    async CriarAsync(data) {
        try {
            const response = await HTTPClient.post('/venda', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar venda:', error);
            throw error;
        }
    },

    // PATCH — atualização parcial, só muda o status para Cancelada
    async CancelarAsync(id, usuarioId) {
        try {
            const response = await HTTPClient.patch(`/venda/${id}/cancelar`, null, { params: { usuarioId } });
            return response.data;
        } catch (error) {
            console.error('Erro ao cancelar venda:', error);
            throw error;
        }
    },
};

export default VendaApi;
