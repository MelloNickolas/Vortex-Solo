import axios from 'axios';

export const HTTPClient = axios.create({
    baseURL: 'http://localhost:5056/api',
    headers: {
        'Content-Type': 'application/json;charset=UTF-8',
    },
});

// Interceptor de requisição, ele vai executa ANTES de cada requisição sair do front
// Injeta o token JWT no header Authorization automaticamente
HTTPClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');

    // Se existir token, adiciona no header ai o backend espera ====== Authorization: Bearer SEU_TOKEN
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

// Interceptor de resposta, quando a API retorna 401 (token expirado ou inválido)
// redireciona para o login e limpa o token salvo
HTTPClient.interceptors.response.use(
    (response) => response,

    (error) => {
        // Só redireciona para /login se já havia um token salvo (token expirou)
        // Se não tinha token, é tentativa de login com credenciais erradas — deixa o erro chegar na página
        const token = localStorage.getItem('token');
        if (error.response?.status === 401 && token) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }

        return Promise.reject(error);
    }
);
