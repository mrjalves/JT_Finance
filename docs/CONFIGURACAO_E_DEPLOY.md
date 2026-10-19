# ⚙️ Guia de Configuração de Ambiente e Implantação (Deploy) — JT_Finance

Este guia orienta o desenvolvedor ou operador de infraestrutura sobre como configurar, parametrizar e implantar o projeto **JT_Finance** em ambientes locais e de produção.

---

## 1. Pré-requisitos de Desenvolvimento

| Ferramenta | Versão Recomendada | Finalidade |
|---|---|---|
| **.NET SDK** | 10.0 ou superior | Compilação e execução de todos os projetos |
| **Visual Studio** | 2024 / 2026 (v18+) ou VS Code com C# Dev Kit | IDE de desenvolvimento |
| **Workload .NET MAUI** | .NET 10 MAUI Workload | Compilação das versões Mobile (Android/iOS) e Desktop (Windows) |
| **Android SDK & Emulador** | API Level 34+ | Execução e testes na plataforma Android |
| **Git** | 2.40+ | Controle de versão |

Para instalar o workload MAUI via terminal:
```bash
dotnet workload install maui
```

---

## 2. Estrutura de Configuração (`appsettings.json`)

O arquivo `appsettings.json` localiza-se nos projetos `JT_Finance.Web` e embutido como recurso em `JT_Finance` (MAUI).

### Principais Seções de Configuração:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Config": {
    "Conexao": {
      "TipoConexao": "API",
      "BaseApiUrl": "https://api.jtecsistemas.com.br",
      "TipoBanco": "SqlServer",
      "StringConexao": "Data Source=...;Initial Catalog=JT_Finance;Encrypt=True;..."
    },
    "Ambiente": "Desenvolvimento",
    "Executor": "Sistema",
    "LogAtivo": true,
    "DiretorioLogs": "Logs",
    "TimeoutPadrao": 30,
    "Nome": "JTech Finance",
    "Versao": "1.0.0",
    "TipoApp": "WEB",
    "FirebaseConfig": {
      "ApiKey": "AIzaSy...",
      "DatabaseUrl": "https://api.jtecsistemas.com.br",
      "BaseApiUrl": "https://api.jtecsistemas.com.br",
      "ProjetoID": "controlefinanceiro-35e2a",
      "AuthorizedDomains": [
        "localhost",
        "127.0.0.1",
        "jt-finance.web.app",
        "10.0.2.2"
      ],
      "GoogleClientIdAndroid": "521428017393-...apps.googleusercontent.com",
      "GoogleClientIdWindows": "521428017393-...apps.googleusercontent.com",
      "GoogleRedirectUriAndroid": "com.googleusercontent.apps.521428017393-...:/oauth2redirect"
    }
  }
}
```

> [!IMPORTANT]
> Em produção, credenciais e chaves privadas de certificados (como a conta de serviço do Firebase) nunca devem ser versionadas em repositórios públicos. Utilize variáveis de ambiente ou o arquivo `appsettings.Production.json` mantido diretamente no servidor.

---

## 3. Executando o Projeto Localmente

### 3.1. Versão Web (ASP.NET Core / Blazor Server + WASM)
No terminal do projeto:
```bash
cd JT_Finance/JT_Finance.Web
dotnet run
```
Acesse no navegador:
- HTTP: `http://localhost:5000` ou conforme `launchSettings.json`
- HTTPS: `https://localhost:7166`

### 3.2. Versão Desktop Windows (.NET MAUI)
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0 JT_Finance/JT_Finance/JT_Finance.csproj
```

### 3.3. Versão Android (.NET MAUI)
Certifique-se de que o emulador Android esteja iniciado ou um dispositivo físico esteja conectado via USB com depuração ativada:
```bash
dotnet build -t:Run -f net10.0-android JT_Finance/JT_Finance/JT_Finance.csproj
```

> [!TIP]
> **Acesso Localhost no Android**: O emulador Android roda dentro de uma máquina virtual com IP próprio. O `MauiProgram.cs` já ajusta automaticamente chamadas com `localhost` para `10.0.2.2`, que é o endereço do host no ambiente de emulação Android.

---

## 4. Endpoints Minimal APIs do Servidor Web

O projeto `JT_Finance.Web` expõe endpoints de gerenciamento que podem ser consumidos pelo cliente WebAssembly ou ferramentas de integração:

### Configurações Financeiras:
- `GET /api/configuracoes-financeiras`: Retorna todas as configurações.
- `GET /api/configuracoes-financeiras/ativa`: Retorna a configuração ativa atual.
- `GET /api/configuracoes-financeiras/{id}`: Retorna a configuração pelo identificador.
- `POST /api/configuracoes-financeiras`: Cria uma nova configuração financeira.
- `PUT /api/configuracoes-financeiras/{id}`: Atualiza uma configuração financeira existente.

### Configurações de Inteligência Artificial:
- `GET /api/configuracoes-ia`: Lista parâmetros cadastrados de IA.
- `GET /api/configuracoes-ia/modelos?apiKey={key}`: Consulta modelos de IA disponíveis pelo provedor.
- `POST /api/configuracoes-ia` & `PUT /api/configuracoes-ia/{id}`: Persiste parâmetros de conexão com LLMs.

### Configurações de Hotkeys & Acessos:
- `GET/POST/PUT /api/configuracoes-hotkeys`: Gerenciamento dos atalhos de teclado globais.
- `GET/POST/PUT /api/configuracoes-acessos`: Gerenciamento de permissões de módulos.

---

## 5. Pipeline de Integração e Entrega Contínua (CI/CD)

O repositório possui uma esteira automatizada configurada via **GitHub Actions** em [.github/workflows/deploy.yml](file:///c:/Users/sirja/source/repos/mrjalves/JT_Finance/.github/workflows/deploy.yml).

### Fluxo do Workflow:
1. **Gatilho**: Disparado automaticamente em cada `push` nas branches `main` e `APP` ou manualmente via `workflow_dispatch`.
2. **Resolução de Dependências Compartilhadas**:
   - Clona o repositório irmão `mrjalves/JT_ApplicationLayer` para fornecer os componentes e serviços base (`JT_UserInterface.Shared`, `JT_Aplicacao`, `JT_Classes`, etc.).
   - Utiliza a Secret `GH_PAT` (Personal Access Token com escopo `repo`).
3. **Build & Publish**:
   - Utiliza o SDK .NET 10.0 (`10.0.x`).
   - Compila o projeto web `JT_Finance/JT_Finance.Web/JT_Finance.Web.csproj` em modo `Release`.
   - Gera o pacote de publicação na pasta `./publish/`.
4. **Comunicação Segura (SSH)**:
   - Configura as chaves privadas do servidor remoto a partir das Secrets do GitHub:
     - `GH_PAT`: Token de acesso para clonar repositórios privados da organização/usuário.
     - `VPS_SSH_KEY`: Chave privada SSH.
     - `VPS_HOST`: Endereço IP ou hostname do servidor.
     - `VPS_PORT`: Porta SSH de conexão.
     - `VPS_USER`: Usuário do servidor Linux.
     - `VPS_PATH`: Caminho de destino da aplicação no servidor (padrão: `/var/www/jt_sistemas_finance`).
5. **Sincronização com `rsync`**:
   - Transfere os arquivos compilados excluindo arquivos sensíveis e dados de produção:
     - `--exclude 'appsettings.json'`
     - `--exclude 'appsettings.Production.json'`
     - `--exclude '*.db*'`
     - `--exclude 'uploads/'`
6. **Reinicialização do Serviço no Linux**:
   - Executa via SSH:
     ```bash
     sudo systemctl restart jt_sistemas_finance
     ```
   - Valida se o serviço está ativo (`systemctl is-active --quiet`) e exibe os logs de inicialização e migração.

---

## 6. Checklist de Resolução de Problemas (Troubleshooting)

| Sintoma | Causa Mais Provável | Ação Recomendada |
|---|---|---|
| **Erro de CORS ou Pop-up no login com Google** | Cabeçalho `Cross-Origin-Opener-Policy` não configurado | Verificar se o middleware de cabeçalhos no `Program.cs` contém `same-origin-allow-popups`. |
| **Falha ao conectar no backend pelo emulador Android** | Tentativa de conexão em `localhost` em vez de `10.0.2.2` | Confirmar que `MauiProgram.cs` está aplicando a substituição automática para a plataforma Android. |
| **401 Unauthorized após expiração de sessão** | Token JWT expirou no `ISecureStorageService` | O sistema redireciona automaticamente para `/login?returnUrl=...`. Faça um novo login para renovar os tokens. |
| **Gráficos não renderizam** | Falha de carregamento do script do Chart.js | Confirmar se o script de suporte a gráficos está referenciado no `App.razor` ou no `index.html`. |
| **Serviço não inicia no servidor Linux** | Porta em uso ou permissões de diretório no `systemd` | Executar `sudo journalctl -u jt_sistemas_finance -e` para inspecionar o log detalhado de inicialização. |
