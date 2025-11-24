# Plano de Testes de Usabilidade

O propósito deste teste é verificar a performance alcançada pelos participantes e o entendimento das funções utilizadas no sistema, com a finalidade de realizar alterações necessárias.

## Definição do(s) objetivo(s)

- Avaliar acessibilidade na plataforma para diferentes perfis de usuários.
- Identificar barreiras na navegação e interação com o sistema.
- Avaliar a eficiência e a satisfação do usuário ao utilizar a interface.
- Verificar se os usuários entendem como se cadastrar na plataforma.
- Avaliar se o fluxo de doação está claro.
- Medir se o tempo para encontrar e se candidatar a uma vaga é aceitável.


## Seleção dos participantes

- Julia Medina (idade: 21 anos; perfil: média familiaridade);
- Lucineia Souza (idade: 57 anos; perfil: básica familiaridade);
- Alexandre Lara (idade: 51 anos; perfil: média familiaridade);
- Luis Frederico Modica (idade: 26 anos; perfil: alta familiaridade);
- Alfredo Paulo (idade: 71 anos; perfil: baixa familiaridade);
- Rodrigo Gurgel (idade: 28 anos; perfil: alta familiaridade);
- Ana Carolina Prates (idade: 16 anos; perfil: média familiaridade);
- Miriam Pacheco (idade: 46 anos; perfil: alta familiaridade);
- Genilda Feliciano (idade: 40 anos; perfil: básica familiaridade);
- Luiz Carlos Sobral Neto (idade: 35 anos; perfil: alta familiaridade);
- Luiz Carlos Sobral Junior (idade: 62 anos; perfil: média familiaridade);
- Juliana Pacheco (Idade: 36 anos; perfil: baixa familiaridade);


## Definição de cenários de teste

**Cenário 1: Doação como usuário externo (sem cadastro)**

**Objetivo:** Validar se um doador sem conta consegue realizar uma doação de forma clara e rápida, preenchendo o formulário no fluxo.

**Contexto:** Usuário acessa a página pública e deseja doar para uma ONG específica.

**Tarefa(s):** 
- Acessar a aplicação com link público
- Clicar na aba “Doações”
- Procurar ONG por nome ou categoria
- Selecionar “Fazer doação”
- Preencher formulário (sem criar conta)
- Selecionar tipo/quantidade da doação (ou valor)
- Confirmar a doação e visualizar a confirmação

**Critério(s) de Sucesso(s):**
-	Concluir a doação em ≤ 3 minutos.
-	Zero bloqueios críticos (ex.: formulário não envia, erro sem mensagem clara).
-	Campos obrigatórios apontam erros claros e no local certo.

**Cenário 2: Cadastro de ONG**

**Objetivo:** Verificar se uma nova ONG consegue se cadastrar e enviar dados para análise.

**Contexto:** Usuário cria conta (perfil ONG) e completa dados.

**Tarefa(s):** 
- Iniciar cadastro de ONG e criar credenciais
- Preencher informações institucionais (endereço, descrição, CNPJ, contato)
- Anexar documentações necessárias para validação
- Enviar para validação e ver estado “Pendente”
- Confirmar que a ONG mudou para estado “Aprovada” e recebeu notificação após validação do ADMIN

**Critério(s) de Sucesso(s):**
-	ONG conclui cadastro em ≤ 5 minutos sem dúvidas sobre campos.
-	Estados visíveis: Pendente > Aprovada.
-	Notificação (ou status em tela) é clara e imediata.

**Cenário 3: Criação de meta e vinculação de doações a uma meta**

**Objetivo:** Verificar se o “gestor” da ONG entende e consegue criar Metas (ex.: “100 cestas”) e se doações podem ser vinculadas a uma meta específica.

**Contexto:** ONG deseja criar uma meta pública para mobilizar doações.

**Tarefa(s):** 
- Fazer login na conta da ONG
- Acessar página de perfil
- Clicar na aba “recursos” e “criar nova meta”
- Preencher dados e solicitações da meta
- Verificar a publicação da meta
- Ver o progresso da Meta atualizado (ex.: “20/100”)

**Critério(s) de Sucesso(s):**
-	Criar meta em ≤ 2 minutos.
-	Na doação, opção de selecionar a meta é evidente.
-	Progresso atualiza em tempo real ou após refresh com mensagem de sucesso.

**Cenário 4: Inscrição de usuário em uma vaga de voluntariado**

**Objetivo:** Validar se o fluxo de candidatura (formulário de inscrição) é intuitivo.

**Contexto:** Candidato visualiza uma vaga e se inscreve, depois a ONG avalia e aprova (virando voluntário).

**Tarefa(s):** 
- Acessar a plataforma com link público
- Clicar na aba “Voluntariado”
- Escolher uma vaga e se inscrever (preenchendo o formulário de inscrição)
- Enviar formulário para aprovação
- Ver confirmação de que está como Candidato pendente
- Verificar se após a aprovação, o usuário recebe uma notificação ou contato da ONG informando aprovação/recusa

**Critério(s) de Sucesso(s):**
-	Fácil acesso ao formulário de preenchimento da vaga.
-	Preenchimento de formulário realizado em ≤ 5 minutos.

**Cenário 5: Doação de Meta**

**Objetivo:** Validar se o fluxo de aprovação de doação é compreensível (para a ONG).

**Contexto:** Doador se inscreve para a vaga e a ONG aprova/reprova a doação.

**Tarefa(s):** 
- Acessar a plataforma com link público
- Realizar login como ONG
- Acessar inscrições pendentes (na aba perfil)
- Aprovar o candidato → o sistema transforma esse registro em um Voluntário (nova entidade herdada)
- Confirmar que os dados de voluntário aparecem na vaga atribuída

**Critério(s) de Sucesso(s):**
- O botão/ação aprovar candidato é compreensível (sem confusão entre “inscrito” e “voluntário”).
- Transição de Candidato para Voluntário ocorre sem perda de dados originais.



## Métodos de coleta de dados

**1. Método Qualitativo (para entender o "Porquê")**
-	Entrevista (Debriefing): Conversar com o usuário após as tarefas ou no final do teste para coletar suas opiniões, percepções de dificuldade e sugestões de melhoria.
  
**2. Método Quantitativo (para medir "O Quê" e "Quanto")**
-	Questionário Padronizado: SUS (System Usability Scale): Um questionário de 10 itens que gera uma pontuação de 0 a 100 sobre a percepção geral da usabilidade.
