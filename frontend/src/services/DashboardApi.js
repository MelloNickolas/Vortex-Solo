import { HTTPClient } from './client';

const DashboardApi = {
    // Faturamento por dia de um mês/ano (filtro server-side)
    async VendasPorDiaAsync(mes, ano) {
        try {
            const response = await HTTPClient.get('/dashboard/vendas-por-dia', { params: { mes, ano } });
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar vendas por dia:', error);
            throw error;
        }
    },
};

export default DashboardApi;
