# Plano de Testes de Software

Os testes funcionais a serem realizados na aplicação são descritos a seguir:

| **Caso de Teste** | **CT01 - Teste da Página de Marketplace** |
|:---:	|:---:	|
|	**Requisitos Associados**  | RF-05 |
| **Objetivo do Teste**  | Verificar se o Usuário (externo ou cadastrado) consegue acessar a página do marketplace, todos os ícones-botões estão funcionais, ícones que levam para outras páginas fazem o percurso correto. Verificar se o filtro está funcional cada opção. Verificar se o modal aparece corretamente e está funcional, se os links levam para as páginas corretas e se as imagens estão carregando. |
| **Passos** 	| 1.Acessar a página por um link externo; <br> 2. Veja se todos os conteúdos são carregados e as imagens também aparecem;  <br> 3.Verifique se todos os botões estão funcionais, clique neles;  <br> 4. Veja se os botões links que levam para outras páginas estão indo para as devidas páginas;  <br> 5. Aplique o filtro, faça várias seleções separas uma para cada um, veja se aparece somente o conteúdo de acordo com filtro selecionado;  <br> 6. Faça seleções múltiplas no filtro e veja se aparece somente o conteúdo que foi selecionado;   <br> 7. Selecione uma possível doação que deseja, clique em faça doação;  <br> 8. Veja se o modal aparece corretamente para realizar a doação;  <br> 9.Coloque um valor dentro do campo de doação; <br> 10. Veja se encaminha para um modal formulário para doação;  <br> 11. Clique no ícone de cópia e cola dentro do modal;  <br> 12.O número do pix é copiado; 1.Acessar a página por um link externo; |
|**Critérios de Êxito** | O usuário acessa a página com êxito, todos os conteúdos aparecem com imagens, os filtros funcionam, botões estão funcionais, e o ao clicar em doar aparece o modal para doação. Usuário consegue preencher os campos e finalizar a ação. |
|  **Responsável pela elaboração do caso de teste**	| Matheus Feliciano Andrade bernardes |

<br>

| **Caso de Teste** 	| **CT02 – Teste da página de Cadastro (ONG)** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar o cadastro da ONG |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Cadastro”; <br> 04.	Em cadastro selecionar o tipo do cadastro a ser realizado (ONG); <br> 05.	Realizar o preenchimento de dados corretamente; <br> 06.	Clicar em “Próximo”; <br> 07.	Visualizar a mensagem “Cadastro pendente de aprovação” (ONG); <br> 08.	Aguardar aprovação do ADMIN; <br> 09.	Receber notificação informando aprovação do cadastro;|
|**Critérios de Êxito** | O usuário consegue realizar o cadastro sem dificuldades; O usuário finaliza o cadastro e recebe a mensagem informando pendência de cadastro; após a aprovação do cadastro o usuário recebe a notificação de conta aprovada |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>


| **Caso de Teste** 	| **CT03 – Teste da página de Login** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-01 e RF-02 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar seu login na plataforma sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Entrar”; <br> 03.	Na página “Como deseja entrar?” selecionar “Login”; <br> 4.	Usuário é redirecionado para a página de login; <br> 05.	Preencher os dados de acesso (e-mail e senha), clicar em “Próximo”; <br> 06.	Verificar aparecimento de mensagem de erro caso os dados sejam preenchidos incorretamente; <br> 07.	Receber a notificação de autenticação (Windows Autenticathor) e responder corretamente; <br> 08.	Usuário é redirecionado para a homepage “logado”;|
|**Critérios de Êxito** | Usuário conseguiu realizar o login sem dificuldade; O usuário conseguiu “responder” a autenticação sem dificuldades; usuário visualizou a mensagem de erro de preenchimento; usuário foi logado|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT04 – Página Homepage do ADMIN** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-09, RF-10, RF-11, RF-12, RF-13, RF-14 |
| **Objetivo do Teste** 	| Verificar se a página inicial de ADMIN funciona corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Login”; <br> 03.	Aguardar carregamento da página; <br> 04.	Preencher o dados de acordo com a conta e clicar em "Fazer login"; <br> 05.	Verificar redirecionamento correto para página de administrador; <br> 06. Verificar se todos os campos (Convidar novo administrador, Cadastros pendentes de aprovação, Administrar e relatórios) estão funcionais.|
|**Critérios de Êxito** | Ao logar como administrador o usuário tem acesso aos dados de ongs cadastradas, consegue criar e enviar um convite de administrador, consegue visualizar, aprovar ou reprovar ongs, consegue administrar a página Quem somos e Fale conosco e consegue filtrar e gerar um relatório com informações pertinentes. |
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT05 – Página Homepage da ONG**	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-04, RF-15 |
| **Objetivo do Teste** 	| Verificar se a página inicial da ONG funciona corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Na homepage clicar em “Login”; <br> 03.	Aguardar carregamento da página; <br> 04.	Preencher o dados de acordo com a conta e clicar em "Fazer login"; <br> 05.	Verificar redirecionamento correto para página da ONG; <br> 06. Verificar se todos os campos Adicionar nova vaga/meta, editar ou excluir vagas/metas existentes, estão funcionais.|
|**Critérios de Êxito** | Ao logar como ONG o usuário tem acesso aos dados da ONG, consegue criar, editar ou excluir uma meta/vaga. |
|  **Responsável pela elaboração do caso de teste**	|  Mariana Turibio Gressi	|

<br>

| **Caso de Teste** 	| **CT06 – Página do perfil da ONG (visão de usuário externo)** 	|
|:---:	|:---:	|
|	**Requisito Associado** 	| RF-01, RF-04, RF-06, RF-10 |
| **Objetivo do Teste** 	| Verificar se a página de perfil de ONGS funciona corretamente |
| **Passos** 	| - Acessar a página de uma ONG <br> - Verificar se as informações da ONG aparecem corretamente (localidade, nome, descrição, tags)  <br> - Verificar se as tabs funcionam corretamente (sobre, vagas de voluntariado, metas de doações) <br> - Verificar se ao clicar em "aplicar" em uma vaga, o modal de inscrição abre corretamente <br> - Verificar se ao clicar em "doar" em um recurso, o modal de doação abre corretamente |
|**Critério de Êxito** | - As informações aparecem corretamente na página, e os modais funcionam corretamente |
|  **Responsável pela elaboração do caso de teste**	|  Mariana Turibio Gressi	|

<br>

| **Caso de Teste** 	| **CT07 – Página do perfil da ONG (visão da ONG)** 	|
|:---:	|:---:	|
|	**Requisito Associado** 	| RF-01, RF-04, RF-06, RF-10 |
| **Objetivo do Teste** 	| Verificar se a página de perfil de ONGS funciona corretamente |
| **Passos** 	|01.	Acessar a aplicação por link público; <br> 02.	Fazer login na aplicação como ONG; <br> 03. Aguardar carregamento da Homepage; <br> 04. Clicar no ícone de perfil; <br> 05. Aguardar redirecionamento para a página de perfil da ONG; <br> 06. Verificar se é possível alterar os dados da ONG (Foto de perfil e de fundo, nome, endereço, contato, descrição, sobre etc.), verificar se consegue gerar relatórios com filtro, verificar se é possível visualizar, editar, adicionar ou excluir filiais;|
|**Critério de Êxito** | - As informações aparecem corretamente na página e o usuário consegue acessar todos os campos funcionais de forma intuitiva |
|  **Responsável pela elaboração do caso de teste**	| Mariana Turibio Gressi	|

<br>

| **Caso de Teste** 	| **CT08 – Teste da página de ONGs cadastradas** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-19 |
| **Objetivo do Teste** 	| Verificar se o usuário (externo ou cadastrado) consegue acessar a página com as ONGs cadastradas na aplicação corretamente |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02.	Clicar na aba "ONGs" no cabeçalho da aplicação; <br> 03. Aguardar carregamento da página de ONGs cadastradas na aplicação; <br> 04. Selecionar uma ONG; <br> 05. Aguardar redirecionamento para a página de perfil da ONG selecionada;|
|**Critérios de Êxito** | O usuário consegue acessar a aba ONGs sem dificuldades; Ao selecionar uma ONG o usuário é redirecionado para a página de perfil da ONG|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica	|

<br>

| **Caso de Teste** 	| **CT09 – Teste do Menu Sobre Nós** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| Verificar se ao passar o mouse no Menu “Sobre nós” e clicar em uma das opções disponíveis será redirecionado respectivamente para a página correta.|
| **Passos** 	| 01.	Passar o mouse no menu “Quem somos”; <br> 02.	Clique em uma das opções disponíveis; <br> 03. Observar se a aplicação o levou para a página correta. |
|**Critérios de Êxito** | O usuário deve conseguir clicar em uma das 2 opções disponíveis. Ao clicar, a aplicação deve redirecioná-lo à página correta.|
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |

<br>

| **Caso de Teste** 	| **CT10 – Teste da Página Fale Conosco** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| Verificar se ao clicar em "Fale conosco" localizado no menu "Sobre nós" a página abre corretamente e e está funcional|
| **Passos** 	| 01.	Clicar em "Fale Conosco" no menu "Sobre nós"; <br> 02. Verificar se a página abre corretamente; <br> 03. Realizar o preenchimento das informações do formulário; <br> 04. Realizar o envio do formulário; <br> 05. Verificar o aparecimento de mensagem de êxito.  |
|**Critérios de Êxito** | O usuário deve conseguir preencher o formulário e enviá-lo sem dificuldades.|
|  **Responsável pela elaboração do caso de teste**	|  Maria Cecilia Caruzzo Modica |

<br>

| **Caso de Teste** 	| **CT11 – Teste da página de redefinição de senha** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| RF-03 |
| **Objetivo do Teste** 	| Verificar se o usuário consegue realizar a redefinição de senha sem dificuldades |
| **Passos** 	| 01.	Acessar a aplicação por link público; <br> 02. Na homepage clicar em “Login”; <br> 03. Usuário é redirecionado para a página de login; <br> 04.	Na página de login selecionar "Esqueceu a senha?" <br> 05. Aguardar redirecionamento para página de redefinição de senha; <br> 06. Preendher o campo com o e-mail cadastrado; <br> 07. Verificar recebimento de e-mail com o link para redefinição de senha e acessá-lo; <br> 08.Preencher dados de acesso corretamente (com a nova senha) clicar em Redefinir e aguardar mensagem de êxito; <br> 10. Clicar novamente em Login e entrar com a nova senha;|
|**Critérios de Êxito** | Usuário consegue redefinir sua senha sem dificuldades|
|  **Responsável pela elaboração do caso de teste**	|  Breno Marques de Moura	|

<br>

| **Caso de Teste** 	| **CT12 – Teste Barra de pesquisa e resultado de pesquisa** 	|
|:---:	|:---:	|
|	**Requisitos Associados** 	| -- |
| **Objetivo do Teste** 	| No homepage logado e não logado, verificar se ao digitar na barra de Pesquisa a página levará o usuário às opções disponíveis com o conteúdo digitado. Verificar se o Usuário (externo ou cadastrado) consegue acessar a página de resultado, aparece todas as opções de acordo com a busca, se ele escolher o conteúdo e clicar irá ser encaminhado para a página que escolheu.|
| **Passos** 	| 01.	Clique no campo "Pesquisar"; <br> 02.	Digite o conteúdo desejado; <br> 03. Clique em "Enter" no seu teclado ou no ícone da lupa. <br> 04. Verificar os conteúdos da página de resultado de pesuisa. <br> 05. Verifique se os conteúdos e suas imagens estão aparecendo; <br> 06.Selecione os ícones dos conteúdos que aparece na tela; <br> 07. Verifique se estão encaminhando para as páginas devidas; |
|**Critérios de Êxito** | O usuário deve conseguir escrever o conteúdo que deseja pesquisar e, em seguida, a aplicação deve redicioná-lo a esse conteúdo disponível. |
|  **Responsável pela elaboração do caso de teste**	|  Márcia Maria dos Reis Marques |




