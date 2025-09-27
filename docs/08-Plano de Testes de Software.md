# Plano de Testes de Software

| **Caso de Teste** 	| **CT01 – Página de ONG** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-01, RF-04, RF-06, RF-10 |
| Objetivo do Teste 	| Verificar se a página de perfil de ONGS funciona corretamente |
| Passos 	| - Acessar a página de uma ONG <br> - Verificar se as informações da ONG aparecem corretamente (localidade, nome, descrição, tags)  <br> - Verificar se as tabs funcionam corretamente (sobre, voluntariado, recursos) <br> - Verificar se ao clicar em "aplicar" em uma vaga, o modal de inscrição abre corretamente <br> - Verificar se ao clicar em "doar" em um recurso, o modal de doação abre corretamente |
|Critério de Êxito | - As informações aparecem corretamente na página, e os modais funcionam corretamente |
|  	|  	|

| **Caso de Teste** 	| **CT02 – Teste da página "Como deseja entrar?"** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se ao clicar em “login” ou “cadastro” a página redireciona corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Aguardar carregamento da página; <br> 04.	Selecionar “login” ou “cadastro”; <br> 05.	Verificar redirecionamento correto para página login ou cadastro;|
|**Critérios de Êxito** | Ao clicar em “login” ou “cadastro” o usuário é corretamente redirecionado para a página desejada em tempo razoável |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT03 – Teste da página de Cadastro (ONG)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar o cadastro da ONG |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Cadastro”; <br> 04.	Em cadastro selecionar o tipo do cadastro a ser realizado (ONG); <br> 05.	Realizar o preenchimento de dados corretamente; <br> 06.	Clicar em “Próximo”; <br> 07.	Visualizar a mensagem “Cadastro pendente de aprovação” (ONG); <br> 08.	Aguardar aprovação do ADMIN; <br> 09.	Receber notificação informando aprovação do cadastro;|
|**Critérios de Êxito** | O usuário consegue realizar o cadastro sem dificuldades; O usuário finaliza o cadastro e recebe a mensagem informando pendência de cadastro; após a aprovação do cadastro o usuário recebe a notificação de conta aprovada |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT04 – Teste da página de Cadastro (ADMIN)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar o cadastro de ADMIN |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Cadastro”; <br> 04.	Em cadastro selecionar o tipo do cadastro a ser realizado (ADMIN); <br> 05.	Realizar o preenchimento de dados corretamente; <br> 06.	Clicar em “Próximo”; <br> 07.	Visualizar a mensagem “Cadastro realizado com sucesso!” (ADMIN); <br> 08.	Usuário é redirecionado para a página de login;|
|**Critérios de Êxito** | O usuário consegue realizar o cadastro sem dificuldades; O usuário finaliza o cadastro e recebe a mensagem informando sucesso; após a aprovação do cadastro o usuário redirecionado para a página de Login|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT05 – Teste da página de Login** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 e RF-02 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar seu login na plataforma sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Preencher os dados de acesso (e-mail e senha), clicar em “Próximo”; <br> 06.	Verificar aparecimento de mensagem de erro caso os dados sejam preenchidos incorretamente; <br> 07.	Receber a notificação de autenticação (Windows Autenticathor) e responder corretamente; <br> 08.	Usuário é redirecionado para a homepage “logado”;|
|**Critérios de Êxito** | Usuário conseguiu realizar o login sem dificuldade; O usuário conseguiu “responder” a autenticação sem dificuldades; usuário visualizou a mensagem de erro de preenchimento; usuário foi logado|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT06 – Teste da página de Gerenciamento de cadastros (ADMIN)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-09 |
| **Objetivo do Teste** 	| Verificar se o ADMIN consegue gerenciar (aprovar/reprovar) o cadastro da ONG sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Preencher os dados de acesso (e-mail e senha), clicar em “Próximo”; <br> 06.	Verificar aparecimento de mensagem de erro caso os dados sejam preenchidos incorretamente; <br> 07.	Responder autenticação; <br> 08.	Acessar a plataforma; <br> 09.	Na homepage “logado” selecionar a opção “Gerenciamento de cadastros”; <br> 10.	Analisar os cadastros pendentes e aprovar ou reprovar;|
|**Critérios de Êxito** | O usuário consegue realizar o gerenciamento de cadastros de ONGs sem dificuldades|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT07 – Teste da página de redefinição de senha** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-03 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar a redefinição de senha sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Na página de login selecionar "Esqueceu a senha?" <br> 06. Aguardar redirecionamento para página de redefinição de senha; <br> 07. Redefinir senha; <br> 08. Aguardar redirecionamento para a página de login; <br> 09.Preencher dados de acesso corretamente (com a nova senha); <br> 10. Clicar em próximo e aguardar redirecioonamento para a página de homepage logado;|
|**Critérios de Êxito** | Usuário consegue realizar a redefinição de sua senha sem dificuldades|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

| **Caso de Teste** 	| **CT08 – Teste da página de ONGs cadastradas** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| Verificar se o usuário (externo ou cadastrado) consegue acessar a página com as ONGs cadastradas na aplicação corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Clicar na aba "ONGs" no cabeçalho da aplicação; <br> 03. Aguardar carregamento da página de ONGs cadastradas na aplicação; <br> 04. Selecionar uma ONG; <br> 05. Aguardar redirecionamento para a página de perfil da ONG selecionada;|
|**Critérios de Êxito** | O usuário consegue acessar a aba ONGs sem dificuldades; Ao selecionar uma ONG o usuário é redirecionado para a página de perfil da ONG|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|
