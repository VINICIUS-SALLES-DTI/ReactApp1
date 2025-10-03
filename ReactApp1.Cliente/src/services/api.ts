import axios from 'axios';

const API_BASE_URL = 'http://localhost:5096/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
  },
});

export interface Material {
  id: number;
  nome: string;
  precoUnitario: number;
  disponivel: boolean;
}

export interface MaterialCriacao {
  nome: string;
  precoUnitario: number;
}

export interface ComponenteDto {
  materialId: number;
  nomeMaterial: string;
  quantidade: number;
  unidadeMedida: string;
}

export interface Traco {
  id: number;
  nome: string;
  resistenciaFck: number;
  slump: number;
}

export interface TracoDetalhes {
  id: number;
  nome: string;
  resistenciaFck: number;
  slump: number;
  componentes: ComponenteDto[];
}

export interface TracoCriacao {
  nome: string;
  resistenciaFck: number;
  slump: number;
  componentes: Array<{
    materialId: number;
    quantidade: number;
  }>;
}

export interface CustoTraco {
  tracoId: number;
  nomeTraco: string;
  custoTotalPorM3: number;
  custoFormatado: string;
}

class ApiService {
  // Métodos para Materiais
  async getMateriais(): Promise<Material[]> {
    const response = await api.get<Material[]>('/Material');
    return response.data;
  }

  async getMaterial(id: number): Promise<Material> {
    const response = await api.get<Material>(`/material/${id}`);
    return response.data;
  }

  async createMaterial(material: MaterialCriacao): Promise<Material> {
    const response = await api.post<Material>('/material', material);
    return response.data;
  }

  async updateMaterial(id: number, material: MaterialCriacao): Promise<void> {
    await api.put(`/material/${id}`, material);
  }

  async deleteMaterial(id: number): Promise<void> {
    await api.delete(`/material/${id}`);
  }

  async searchMateriais(termo: string): Promise<Material[]> {
    const response = await api.get<Material[]>(`/material/pesquisar?termo=${encodeURIComponent(termo)}`);
    return response.data;
  }

  // Métodos para Traços
  async getTracos(): Promise<Traco[]> {
    const response = await api.get<Traco[]>('/traco');
    return response.data;
  }

  async getTraco(id: number): Promise<Traco> {
    const response = await api.get<Traco>(`/traco/${id}`);
    return response.data;
  }

  async getTracoDetalhes(id: number): Promise<TracoDetalhes> {
    const response = await api.get<TracoDetalhes>(`/traco/detalhes/${id}`);
    return response.data;
  }

  async createTraco(traco: TracoCriacao): Promise<Traco> {
    const response = await api.post<Traco>('/traco', traco);
    return response.data;
  }

  async updateTraco(id: number, traco: TracoCriacao): Promise<Traco> {
    const response = await api.put<Traco>(`/traco/${id}`, traco);
    return response.data;
  }

  async deleteTraco(id: number): Promise<void> {
    await api.delete(`/traco/${id}`);
  }

  async calcularCustoTraco(id: number): Promise<CustoTraco> {
    const response = await api.get<CustoTraco>(`/traco/${id}/custo`);
    return response.data;
  }

  async searchTracos(termo: string): Promise<Traco[]> {
    const response = await api.get<Traco[]>(`/traco/pesquisar?termo=${encodeURIComponent(termo)}`);
    return response.data;
  }
}

export const apiService = new ApiService();