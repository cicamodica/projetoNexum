# Plano de Testes de Software

| **Caso de Teste** 	| **CT01 – Página de ONG** 	|
|:---:	|:---:	|
|	**Requisito Associado** 	| RF-01, RF-04, RF-06, RF-10 |
| **Objetivo do Teste** 	| Verificar se a página de perfil de ONGS funciona corretamente |
| **Passos** 	| - Acessar a página de uma ONG <br> - Verificar se as informações da ONG aparecem corretamente (localidade, nome, descrição, tags)  <br> - Verificar se as tabs funcionam corretamente (sobre, voluntariado, recursos) <br> - Verificar se ao clicar em "aplicar" em uma vaga, o modal de inscrição abre corretamente <br> - Verificar se ao clicar em "doar" em um recurso, o modal de doação abre corretamente |
|**Critério de Êxito** | - As informações aparecem corretamente na página, e os modais funcionam corretamente |
|  **Responsável pela elaboração do caso de teste**	|  Erison Guimarães dos Santos	|

<br>

| **Caso de Teste** 	| **CT02 – Teste da página "Como deseja entrar?"** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se ao clicar em “login” ou “cadastro” a página redireciona corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Aguardar carregamento da página; <br> 04.	Selecionar “login” ou “cadastro”; <br> 05.	Verificar redirecionamento correto para página login ou cadastro;|
|**Critérios de Êxito** | Ao clicar em “login” ou “cadastro” o usuário é corretamente redirecionado para a página desejada em tempo razoável |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT03 – Teste da página de Cadastro (ONG)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar o cadastro da ONG |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Cadastro”; <br> 04.	Em cadastro selecionar o tipo do cadastro a ser realizado (ONG); <br> 05.	Realizar o preenchimento de dados corretamente; <br> 06.	Clicar em “Próximo”; <br> 07.	Visualizar a mensagem “Cadastro pendente de aprovação” (ONG); <br> 08.	Aguardar aprovação do ADMIN; <br> 09.	Receber notificação informando aprovação do cadastro;|
|**Critérios de Êxito** | O usuário consegue realizar o cadastro sem dificuldades; O usuário finaliza o cadastro e recebe a mensagem informando pendência de cadastro; após a aprovação do cadastro o usuário recebe a notificação de conta aprovada |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>


| **Caso de Teste** 	| **CT04 – Teste da página de Login** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 e RF-02 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar seu login na plataforma sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Preencher os dados de acesso (e-mail e senha), clicar em “Próximo”; <br> 06.	Verificar aparecimento de mensagem de erro caso os dados sejam preenchidos incorretamente; <br> 07.	Receber a notificação de autenticação (Windows Autenticathor) e responder corretamente; <br> 08.	Usuário é redirecionado para a homepage “logado”;|
|**Critérios de Êxito** | Usuário conseguiu realizar o login sem dificuldade; O usuário conseguiu “responder” a autenticação sem dificuldades; usuário visualizou a mensagem de erro de preenchimento; usuário foi logado|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT05 – Teste da página de Gerenciamento de cadastros (ADMIN)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-09 |
| **Objetivo do Teste** 	| Verificar se o ADMIN consegue gerenciar (aprovar/reprovar) o cadastro da ONG sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Preencher os dados de acesso (e-mail e senha), clicar em “Próximo”; <br> 06.	Verificar aparecimento de mensagem de erro caso os dados sejam preenchidos incorretamente; <br> 07.	Responder autenticação; <br> 08.	Acessar a plataforma; <br> 09.	Na homepage “logado” selecionar a opção “Gerenciamento de cadastros”; <br> 10.	Analisar os cadastros pendentes e aprovar ou reprovar;|
|**Critérios de Êxito** | O usuário consegue realizar o gerenciamento de cadastros de ONGs sem dificuldades|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT06 – Teste da página de redefinição de senha** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-03 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar a redefinição de senha sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Na página de login selecionar "Esqueceu a senha?" <br> 06. Aguardar redirecionamento para página de redefinição de senha; <br> 07. Redefinir senha; <br> 08. Aguardar redirecionamento para a página de login; <br> 09.Preencher dados de acesso corretamente (com a nova senha); <br> 10. Clicar em próximo e aguardar redirecioonamento para a página de homepage logado;|
|**Critérios de Êxito** | Usuário consegue realizar a redefinição de sua senha sem dificuldades|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT07 – Teste da página de ONGs cadastradas** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| Verificar se o usuário (externo ou cadastrado) consegue acessar a página com as ONGs cadastradas na aplicação corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Clicar na aba "ONGs" no cabeçalho da aplicação; <br> 03. Aguardar carregamento da página de ONGs cadastradas na aplicação; <br> 04. Selecionar uma ONG; <br> 05. Aguardar redirecionamento para a página de perfil da ONG selecionada;|
|**Critérios de Êxito** | O usuário consegue acessar a aba ONGs sem dificuldades; Ao selecionar uma ONG o usuário é redirecionado para a página de perfil da ONG|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT08 – Teste da página Homepage logado - Menu** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| No homepage logado, verificar se ao passar o mouse no Menu “Quem somos” abre as opções “Sobre”, “FAQ” e “Fale conosco”. |
| **Passos** 	| 01.	Passar o mouse no menu “Quem somos”; <br> 02.	Observar se aparecem as opções: "Sobre", "FAQ" e "Fale conosco". |
|**Critérios de Êxito** | O usuário deve conseguir ver as opções abaixo do Menu “Quem somos” e, em seguida, deve visualizar e conseguir clicar em todas as 3 opções disponíveis.|
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |

<br>

| **Caso de Teste** 	| **CT09 – Teste da página Homepage logado - Menu Quem somos** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| Verificar se ao passar o mouse no Menu “Quem somos” e clicar em uma das opções disponíveis será redirecionado respectivamente para a página correta.|
| **Passos** 	| 01.	Passar o mouse no menu “Quem somos”; <br> 02.	Clique em uma das opções disponíveis; <br> 03. Observar se a aplicação o levou para a página correta. |
|**Critérios de Êxito** | O usuário deve conseguir clicar em uma das 3 opções disponíveis. Ao clicar, a aplicação deve redirecioná-lo à página correta.|
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |

<br>

| **Caso de Teste** 	| **CT10 – Teste da página Homepage logado - Menu Lateral** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| No homepage logado, conforme o perfil, verificar se ao clicar nos “três traços” (menu lateral) irá abrir as opções disponíveis. |
| **Passos** 	| 01.	Clicar no menu lateral ("três traços"); <br> 02. Observe as opções disponíveis e clique em uma delas. |
|**Critérios de Êxito** | O usuário, após clicar no menu lateral, deve conseguir ver e clicar nas opções disponíveis.|
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |

<br>

| **Caso de Teste** 	| **CT11 – Teste da página Homepage logado e não logado - Pesquisar** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| No homepage logado e não logado, verificar se ao digitar na barra de Pesquisa a página levará o usuário às opções disponíveis com o conteúdo digitado. |
| **Passos** 	| 01.	Clique no campo "Pesquisar"; <br> 02.	Digite o conteúdo desejado; <br> 03. Clique em "Enter" no seu teclado ou no ícone da lupa. |
|**Critérios de Êxito** | O usuário deve conseguir escrever o conteúdo que deseja pesquisar e, em seguida, a aplicação deve redicioná-lo a esse conteúdo disponível. |
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |

<br>

| **Caso de Teste** | **CT12 - Teste da Página de pesquisa** |
|:---:	|:---:	|
|	**Requisitos Associados**  | RF-05 |
| **Objetivo do Teste** 	| Verificar se o Usuário (externo ou cadastrado) consegue acessar a página, realizar pesquisa e ser encaminhado para tela de resultado de pesquisa . |
| **Passos** 	| 1. Acessar a aplicação por link público; <br> 2. Clicar no campo “pesquisa” no cabeçalho da página; <br> 3. Digitar por exemplo “Voluntario “; <br>  4. Aparecer todos os conteúdos relacionados a palavra de busca; <br> 5. Selecionar alguma das sugestões conteúdos e ser redirecionado para tela de resultado de pesquisa; |
|**Critérios de Êxito** | O usuário obtém sucesso ao realizar a pesquisa e a aplicação mostra as sugestões de acordo com a palavra de busca e redireciona após selecionar para a tela de resultado. |
|  **Responsável pela elaboração do caso de teste**	| Matheus Feliciano Andrade bernardes |

<br>

| **Caso de Teste** | **CT13 - Teste da Página de Resultado de pesquisa** |
|:---:	|:---:	|
|	**Requisitos Associados**  | RF-05 |
| **Objetivo do Teste** 	| Verificar se o Usuário (externo ou cadastrado) consegue acessar a página de resultado, aparece todas as opções de acordo com a busca, se ele escolher o conteúdo e clicar irá ser encaminhado para a página que escolheu. |
| **Passos** 	| 1.Após realizar a pesquisa e der Enter , deve aparecer os conteúdos respectivos de acordo com a pesquisa; <br> 2. Veja se aparece os conteúdos e suas imagens estão aparecendo; <br> 3.Selecione os ícones dos conteúdos que aparece na tela; <br> 4. Verifique se estão encaminhando para as páginas devidas; |
|**Critérios de Êxito** | O conteúdo e exibido com êxito de acordo com a pesquisa realizada, as imagens são carregadas, mostra todos os conteúdos de acordo com o contexto pesquisado, todos os ícones levam para as páginas respectivas. |
|  **Responsável pela elaboração do caso de teste**	| Matheus Feliciano Andrade bernardes |

<br>

| **Caso de Teste** | **CT14 - Teste da Página de Marketplace** |
|:---:	|:---:	|
|	**Requisitos Associados**  | RF-05 |
| **Objetivo do Teste** 	| Verificar se o Usuário (externo ou cadastrado) consegue acessar a página do marketplace, todos os ícones-botões estão funcionais, ícones que levam para outras páginas fazem o percurso correto. Verificar se o filtro está funcional cada opção. Verificar se o modal aparece corretamente e está funcional, se os links levam para as páginas corretas e se as imagens estão carregando. |
| **Passos** 	| 1.Acessar a página por um link externo; <br> 2. Veja se todos os conteúdos são carregados e as imagens também aparecem;  <br> 3.Verifique se todos os botões estão funcionais, clique neles;  <br> 4. Veja se os botões links que levam para outras páginas estão indo para as devidas páginas;  <br> 5. Aplique o filtro, faça várias seleções separas uma para cada um, veja se aparece somente o conteúdo de acordo com filtro selecionado;  <br> 6. Faça seleções múltiplas no filtro e veja se aparece somente o conteúdo que foi selecionado;   <br> 7. Selecione uma possível doação que deseja, clique em faça doação;  <br> 8. Veja se o modal aparece corretamente para realizar a doação;  <br> 9.Coloque um valor dentro do campo de doação; <br> 10. Veja se encaminha para um modal formulário para doação;  <br> 11. Clique no ícone de cópia e cola dentro do modal;  <br> 12.O número do pix é copiado; 1.Acessar a página por um link externo; |
|**Critérios de Êxito** | O usuário acessa a página com êxito, todos os conteúdos aparecem com imagens, os filtros funcionam, botões estão funcionais, e o ao clicar em doar aparece o modal para doação. Usuário consegue preencher os campos e finalizar a ação. |
|  **Responsável pela elaboração do caso de teste**	| Matheus Feliciano Andrade bernardes |

<br>

| **Caso de Teste** | **CT15 - Teste da Página de Voluntariado** |
|:---:	|:---:	|
|	**Requisitos Associados**  | RF-05 |
| **Objetivo do Teste** 	| Verificar se o Usuário (externo ou cadastrado) consegue acessar a página de voluntariado, consegue realizar filtragem para acessar apenas conteúdos que deseja, verificar se todos os botões links estão encaminhando para as páginas corretas, se botões estão funcionais é se o usuário consegue fazer aplicação para se voluntariar. |
| **Passos** 	| 1.Acessar a página por um link externo; <br> 2. Veja se todos os conteúdos são carregados e as imagens também aparecem; <br> 3.Verifique se todos os botões estão funcionais, clique neles;   <br> 4. Veja se os botões links que levam para outras páginas estão indo para as devidas páginas;  <br> 5. Aplique o filtro, faça várias seleções separas uma para cada um, veja se aparece somente o conteúdo de acordo com filtro selecionado;  <br> 6. Faça seleções múltiplas no filtro e veja se aparece somente o conteúdo que foi selecionado;  <br> 7.Selecione um possível vaga para se voluntariar clicando em candidatar-se;  <br> 8. Aparece o modal para realizar a aplicação;  <br> 9.Prencher os campos necessarios; <br> 10. clicar em submeter; |
|**Critérios de Êxito** |O usuário acessa a página com êxito, todos os conteúdos aparecem com imagens, os filtros funcionam, botões estão funcionais, o modal de formulário carrega todos os conteúdos e está funcional e aplicável . |
|  **Responsável pela elaboração do caso de teste**	| Matheus Feliciano Andrade bernardes |




