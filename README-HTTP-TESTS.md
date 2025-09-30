# Arquivos .http para Testes dos Microserviços

Este projeto contém arquivos `.http` para testar os endpoints de ambos os microserviços (Proposta.Service e Contratacao.Service).

## 📁 Estrutura dos Arquivos

### 1. **Proposta.Service.API.http**
Localização: `/Proposta.Service/Proposta.Service.API/Proposta.Service.API.http`

**Endpoints disponíveis:**
- `POST /propostas` - Criar nova proposta
- `GET /propostas/{id}` - Obter proposta por ID
- `GET /propostas?pagina={}&tamanhoPagina={}` - Listar propostas com paginação
- `PUT /propostas/{id}/status` - Atualizar status da proposta

### 2. **Contratacao.Service.API.http**
Localização: `/Contratacao.Service/Contratacao.Service.API/Contratacao.Service.API.http`

**Endpoints disponíveis:**
- `POST /contratar` - Contratar seguro baseado em proposta aprovada

### 3. **integration-tests.http**
Localização: `/integration-tests.http` (raiz do projeto)

**Cenários de teste:**
- ✅ Fluxo completo de sucesso (Local e Docker)
- ❌ Fluxo de falha com proposta rejeitada
- ❌ Fluxo de falha com proposta inexistente

## 🚀 Como Executar

### Usando VS Code
1. Instale a extensão **REST Client** (humao.rest-client)
2. Abra qualquer arquivo `.http`
3. Clique em "Send Request" acima de cada requisição

### Usando Visual Studio
1. Os arquivos `.http` são suportados nativamente
2. Clique no botão de execução ao lado de cada requisição

### Usando outras ferramentas
Copie e cole as requisições em ferramentas como Postman, Insomnia, ou curl.

## 🌐 Configuração de Endpoints

### Desenvolvimento Local
- **Proposta Service**: `http://localhost:5284`
- **Contratacao Service**: `http://localhost:5206`

### Docker
- **Proposta Service**: `http://localhost:5000`
- **Contratacao Service**: `http://localhost:5001`

## 📋 Dados de Teste

### Status da Proposta (StatusPropostaEnum)
- `0` - Em Análise
- `1` - Aprovada
- `2` - Rejeitada

### GUIDs de Teste Pré-definidos
- `11111111-1111-1111-1111-111111111111` - Proposta para fluxo de sucesso
- `22222222-2222-2222-2222-222222222222` - Proposta para Docker
- `33333333-3333-3333-3333-333333333333` - Proposta para teste de rejeição
- `99999999-9999-9999-9999-999999999999` - GUID para teste de "não encontrado"

## 🔄 Fluxo de Teste Recomendado

### Cenário de Sucesso Completo:

1. **Criar Proposta**
   ```http
   POST /propostas
   {
     "id": "11111111-1111-1111-1111-111111111111",
     "segurado": "João Silva",
     "produto": "Seguro Auto",
     "valor": 1500.00,
     "status": 0
   }
   ```

2. **Aprovar Proposta**
   ```http
   PUT /propostas/11111111-1111-1111-1111-111111111111/status
   { "status": 1 }
   ```

3. **Contratar Seguro**
   ```http
   POST /contratar
   { "propostaId": "11111111-1111-1111-1111-111111111111" }
   ```

### Cenário de Falha:

1. **Criar Proposta**
2. **Rejeitar Proposta** (status: 2)
3. **Tentar Contratar** (deve retornar erro)

## 🔧 Solução de Problemas

### Erro de Conexão
- Verifique se os serviços estão rodando nas portas corretas
- Para Docker: `docker-compose up -d`
- Para desenvolvimento local: execute os projetos no Visual Studio/VS Code

### Erro 404 - Proposta não encontrada
- Certifique-se de que a proposta foi criada primeiro
- Verifique se o GUID está correto

### Erro 400 - Não foi possível contratar
- Verifique se a proposta está com status "Aprovada" (1)
- Confirme se o serviço de Proposta está acessível

## 📊 Estrutura de Resposta

### Sucesso (200/201)
```json
{
  "id": "guid",
  "data": "2024-09-30T10:00:00Z",
  "segurado": "Nome",
  "produto": "Tipo de Seguro",
  "valor": 1500.00,
  "status": 1
}
```

### Erro (400/404)
```json
{
  "error": "Mensagem de erro"
}
```

## 🏗️ Arquitetura

O projeto implementa:
- **Padrão Saga** para orquestração entre microserviços
- **Arquitetura Hexagonal** (Ports & Adapters)
- **Comunicação HTTP** entre serviços
- **PostgreSQL** para persistência de dados