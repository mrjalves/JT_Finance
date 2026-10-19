# 📱 Guia de Funcionalidades e Telas — JT_Finance

Este documento fornece um guia funcional completo sobre os módulos, rotas e casos de uso do sistema **JT_Finance**.

---

## 1. Painel Inicial (Dashboard) — Rota: `/home`

O Dashboard é o ponto central de controle financeiro do usuário, atualizado dinamicamente de acordo com o mês de referência selecionado.

```
+-------------------------------------------------------------------------------+
|  < [ Outubro 2026 ] >                                                         |
|                                                                               |
|  [ Total Receitas ]        [ Total Despesas ]          [ Saldo Atual ]        |
|     R$ 8.500,00               R$ 4.250,00                R$ 4.250,00          |
|                                                                               |
|  [ Despesas Vencidas: 1 (R$ 150,00) ]     [ Próximas ao Vencimento: 3 (R$ 820)]|
+-------------------------------------------------------------------------------+
|  [ Gráfico: Despesas por Categoria ]      [ Gráfico: Despesas por Tipo ]      |
|           (Gráfico de Rosca)                          (Gráfico de Barras)     |
+-------------------------------------------------------------------------------+
```

### Recursos da Tela:
1. **Navegador Temporal**: Botões de avanço e retrocesso de meses (`<` e `>`), permitindo auditar meses passados ou planejar lançamentos futuros.
2. **Cards de KPIs**:
   - **Receitas**: Soma total das entradas no mês.
   - **Despesas**: Soma de saídas (gerais + parcelas de cartão).
   - **Saldo do Mês**: Diferença matemática imediata (Receitas - Despesas).
3. **Alertas de Risco**:
   - **Despesas Vencidas**: Mostra quantidade e total em atraso; ao clicar, abre modal com a lista detalhada.
   - **Próximas ao Vencimento**: Destaca compromissos a vencer nos próximos dias.
4. **Gráficos Integrados**:
   - **Despesas por Categoria**: Visualização em rosca (Doughnut) com percentual por categoria (ex.: Alimentação, Moradia, Transporte).
   - **Despesas por Tipo**: Visualização em barras agrupando por tipos de desembolso.

---

## 2. Gestão de Despesas — Rota: `/despesas`

Permite o registro, controle, classificação e auditoria de todas as saídas financeiras.

### Funcionalidades:
- **Grade Dinâmica (`GridDespesas`)**:
  - Exibe Colunas: Descrição, Valor, Data, Data de Vencimento, Categoria, Tipo e Status de Pagamento.
  - Ordenação por cabeçalho em todas as colunas.
  - Busca rápida em tempo real e filtros avançados.
  - Paginação server-side parametrizável pelo perfil do usuário.
- **Rodapé de Totais**:
  - Total de Itens listados.
  - Valor Total consolidado.
  - Somatório de despesas **Pagas** (em verde) e **Pendentes** (em vermelho).
- **Cadastro e Edição (`ModalDespesa`)**:
  - Campos: Descrição, Valor, Data da Despesa.
  - **Suporte a Cartão de Crédito**:
    - Ao ativar o switch de despesa de cartão, o sistema habilita um seletor pesquisável (`SeletorPesquisa`) para escolher o cartão cadastrado.
    - Definição do número de parcelas. O sistema calcula automaticamente o vencimento de cada parcela com base no dia de fechamento do cartão.
  - **IA Assistiva**: Botão de assistência por IA para sugestão de preenchimento inteligente baseado em históricos anteriores.
- **Ações Rápidas**:
  - **Duplicar Despesa**: Clona uma despesa recorrente com apenas um clique.
  - **Excluir com Diálogo de Confirmação (`ConfirmDialog`)**: Evita exclusões acidentais.
  - **Atalhos de Teclado**: Integração com hotkeys globais para acelerar o lançamento de despesas.

---

## 3. Gestão de Receitas — Rota: `/receitas`

Módulo dedicado à escrituração de entradas monetárias (salários, comissões, dividendos, transferências e vendas).

### Funcionalidades:
- **Grade de Receitas (`GridReceitas`)**:
  - Exibição de descrição, valor, data do recebimento e categoria.
  - Indicadores no rodapé com valor global arrecadado no período.
- **Formulário de Cadastro/Edição (`ModalReceita`)**:
  - Entrada de valor, data e categoria financeira.
- **Duplicação de Lançamento**: Facilidade para replicar proventos mensais recorrentes.

---

## 4. Gestão de Cartões de Crédito — Rota: `/cartoes-credito`

Visão macro e gerencial dos cartões de crédito físicos ou virtuais do usuário.

### Informações por Cartão:
- **Nome/Identificação** do cartão (ex.: *Nubank Ultravioleta*, *Itaú Click*).
- **Limite Total** configurado.
- **Dia de Fechamento**: Data de corte da fatura.
- **Dia de Vencimento**: Data limite para pagamento sem juros.
- **Status**: Ativo / Inativo.
- **Fatura do Mês Atual**: Total lançado para o ciclo de faturamento corrente.
- **Total Geral em Aberto**: Soma de todas as parcelas futuras ainda não quitadas.

### Interações:
- **Duplo Clique na Linha**: Navega diretamente para a fatura detalhada do cartão (`/fatura/{id}`).
- **Botão Ver Faturas**: Atalho visual dedicado para o extrato da fatura.
- **Totalizadores no Rodapé**: Quantidade de cartões ativos/inativos, soma dos limites e valor total comprometido em faturas.

---

## 5. Visualizador & Pagamento de Faturas — Rota: `/fatura/{CartaoId}`

Tela especializada no gerenciamento do ciclo de vida das faturas de cartão.

```
+-------------------------------------------------------------------------------+
|  < Voltar para Cartões       Faturas: Nubank Mastercard                       |
|                                                                               |
|  [ Total em Aberto: R$ 1.840,00 ]          [ Total Pago: R$ 3.200,00 ]        |
+-------------------------------------------------------------------------------+
|  ▼ Fatura com Vencimento: 15/10/2026                 [ Status: Parcial ]       |
|    Total: R$ 940,00 | Pago: R$ 500,00 | Aberto: R$ 440,00                     |
|    [ Pagar Fatura ]                                                           |
|                                                                               |
|    - Supermercado Pão de Açúcar (Parcela 1/3)     R$ 220,00   [ Paga ]        |
|    - Assinatura Streaming                         R$  55,90   [ Pendente ]    |
|    - Posto Ipiranga Combustível                   R$ 164,10   [ Pendente ]    |
+-------------------------------------------------------------------------------+
|  ► Fatura com Vencimento: 15/11/2026                 [ Status: Em Aberto ]    |
+-------------------------------------------------------------------------------+
```

### Funcionalidades:
1. **Agrupamento Mensal**: Cada vencimento de fatura forma um bloco sanfonado (accordion) expansível.
2. **Cálculo de Parcelas**: Mostra a fração exata (ex.: *Parcela 2 de 10*) e o vencimento calculado.
3. **Liquidação de Faturas (`PagarFatura`)**:
   - Permite liquidar as parcelas da fatura informando a data de pagamento efetiva.
   - Atualiza instantaneamente o status das despesas para "Paga", refletindo no saldo da Home e liberando o limite do cartão.
4. **Filtro Temporal por Query Parameters**: Suporta acesso direto a competências específicas via `/fatura/{id}/{ano}-{mes}`.

---

## 6. Construtor de Dashboards & IA — Rotas: `/dashboards` e `/dashboards/{id}`

O **JT_Finance** conta com um construtor de Business Intelligence personalizado, permitindo criar painéis específicos com métricas do Data Warehouse (EDW).

### 6.1. Catálogo de Indicadores Disponíveis
- `totalReceitas`: Card com o total de receitas do mês.
- `totalDespesas`: Card com o total de despesas do mês.
- `saldo`: Card exibindo a diferença entre receitas e despesas.
- `despesasVencidas`: Card com contagem e montante de despesas vencidas.
- `proximasVencimento`: Card com despesas a vencer.
- `despesasPorCategoria`: Gráfico de Rosca com despesas por categoria.
- `despesasPorTipo`: Gráfico de Barras com despesas por tipo.

### 6.2. Assistente IA de Composição (`DashboardIAComposer`)
- O usuário abre o modal de IA e insere uma instrução em texto (ex.: *"Quero um dashboard voltado para redução de gastos, destacando contas vencidas e gastos por categoria"*).
- A IA responde com uma **prévia editável** composta por widgets sugeridos.
- O usuário pode ajustar títulos, tipos de gráficos e ordem antes de consolidar a criação.

### 6.3. Compartilhamento Público por Token (`/compartilhado/{token}`)
- Permite gerar um link público seguro com um token único aleatório.
- **Snapshots Imutáveis**: O dashboard armazena uma foto estática dos dados no momento do compartilhamento (`DashboardIndicadorSnapshot`), permitindo que parceiros, clientes ou consultores visualizem os números sem necessidade de login e sem risco de expor outros dados da conta.

---

## 7. Calendário & Histórico Financeiro — Rota: `/historico`

O módulo de histórico utiliza a visualização em calendário mensal para apresentar o fluxo financeiro ao longo dos dias do mês:
- Cada dia do calendário exibe pequenas tags com os lançamentos de receitas e despesas.
- Diferenciação visual com cores intuitivas (verde para entradas, vermelho para saídas).
- Facilita a identificação de picos de desembolso no mês (dias de vencimento de faturas ou contas fixas).

---

## 8. Listas de Compras — Rota: `/listacompras`

Ferramenta prática para planejamento prévio de gastos no supermercado, feira ou projetos:
- Criação de múltiplas listas independentes.
- Gerenciamento de itens: nome do produto, quantidade, valor previsto e valor real no momento da compra.
- Acompanhamento do subtotal estimado versus realizado para evitar estouro de orçamento.

---

## 9. Configurações & Personalização — Rota: `/configuracoes`

Aba centralizada de configurações financeiras:
- Definição do número padrão de itens por página nas grids.
- Personalização de categorias e tipos de despesa/receita.
- Configuração de atalhos de teclado (Hotkeys).
- Configuração de parâmetros de conexão com provedores de IA (API Keys e modelos LLM).
