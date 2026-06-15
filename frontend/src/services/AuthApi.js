import { HTTPClient } from './client';

const AuthApi = {
    // Envia email e senha, retorna { token, nome, email, expiracao }
    async LoginAsync(data) {
        try {
            const response = await HTTPClient.post('/auth/login', data);
            return response.data;
        } catch (error) {
            console.error('Erro ao fazer login:', error);
            throw error;
        }
    },
};

export default AuthApi;
