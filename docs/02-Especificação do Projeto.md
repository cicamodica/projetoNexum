# Especificações do Projeto

## Personas
 <img src="https://github.com/user-attachments/assets/3fb6c2a3-7400-4563-9d8d-b6f2a639a6ec">
 <img src="https://github.com/user-attachments/assets/f9207163-efd2-448a-a92e-29cb158c42af">
 <img src="https://github.com/user-attachments/assets/0892ab65-cbd7-40dc-b026-1693d8436aab">
 <img src="https://github.com/user-attachments/assets/918ede47-9e56-4679-a342-b5272e54c5c8">
 <img src="https://github.com/user-attachments/assets/c6df001d-cb15-420a-839a-30457dd8ee50">


 
 
 

 

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`     | QUERO/PRECISO ... `FUNCIONALIDADE`                                   |PARA ... `MOTIVO/VALOR`                                                                                              |
|-------------------------|----------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
|Coordenadora de ONG      | Cadastrar demandas de voluntários                                    | Conseguir encontrar pessoas qualificadas rapidamente para minhas atividades                                         |
|Coordenadora de ONG      | Compartilhar equipamentos e materiais                                | Otimizar o uso dos recursos da ONG e ajudar outras instituições próximas                                            | 
|Voluntário Designer      | Visualizar oportunidades disponíveis conforme minhas habilidades e disponibilidade.                         | Poder contribuir de forma eficiente onde minhas competências são necessárias |
|Voluntário Designer      | Filtrar oportunidades por localização e disponibilidade              | Planejar melhor meu tempo e escolher projetos que posso realmente participar                                        |
|ONG Ambiental            | Solicitar empréstimo de recursos                                     | Atender demandas urgentes sem precisar comprar novos equipamentos, reduzindo custos                                 |
|ONG Ambiental            | Quero apoiar outras ONGs                                             | Fortalecer a colaboração e ampliar o impacto coletivo das instituições                                              |
|ONG de pequeno porte     | Encontrar voluntários qualificados                                   | Executar campanhas de cidadania e ampliar o impacto das ações sociais                                               |
|ONG de pequeno porte     | Gerar relatórios de transparência                                    | Apresentar resultados claros e confiáveis a doadores, parceiros e à comunidade, fortalecendo a credibilidade da ONG |
|Doadora Financeira       | Encontrar com facilidade ONGs que necessitem de doações financeiras  | Realizar doações financeiras                                                                                        |
|Doadora Financeira       | Ter fácil acesso a relatórios de transparência das ONGs ajudadas     | Para escolher as ONGs em que acredita que seu dinheiro será bem utilizado                                           |


## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais que detalham o escopo do projeto.

### Requisitos Funcionais

|   ID   | Descrição do Requisito  | Prioridade |
|-------|-------------------------|------------|
|RF-01| Permitir que o usuário (ONG) crie e gerencie seu perfil na plataforma. | ALTA | 
|RF-02| Permitir autenticação de usuários (ONGs) por Login e Logout. | ALTA |
|RF-03| Permitir recuperação/redefinição de senha.  | ALTA |
|RF-04| Permitir que ONGs cadastrem oportunidades de voluntariado e recursos disponíveis.  | ALTA |
|RF-05| Permitir que voluntários e doadores pesquisem causas filtrando por necessidades, localização, área de interesse e disponibilidade.  | ALTA |
|RF-06| Permitir a gestão de recursos e inventário para ONGs. | ALTA |
|RF-07| Gerar relatórios de recursos, inventário, vagas disponibilizadas/preenchidas e candidaturas (ONGs).  | ALTA |
|RF-08| Implementar um sistema de verificação/aprovação para perfis (ONGs).   | ALTA |
|RF-09| O sistema deve permitir que o administrador faça login utilizando e-mail e senha válidos. | ALTA |
|RF-10| O sistema deve permitir que somente usuários com perfil/role de administrador acessem a página de administração. | ALTA |
|RF-11| O sistema deve apresentar ao administrador uma lista de cadastros pendentes de aprovação. | ALTA |
|RF-12| O sistema deve permitir ao administrador aprovar ou reprovar cadastros pendentes. | ALTA |
|RF-13| O sistema deve permitir ao administrador gerar relatórios com base em filtros (data, status, tipo de usuário etc.). | ALTA |
|RF-14| O sistema deve permitir ao administrador editar conteúdos já cadastrados. | MÉDIA |
|RF-15| Disponibilizar um sistema de notificação por e-mail (voluntários) ou push (ONGs).  | MÉDIA |
|RF-16| Permitir avaliação e feedback de usuários sobre o sistema.   | BAIXA |
|RF-17| A aplicação deve permitir que o usuário seja redirecionado por meio do Comece agora para a página Cadastrar ONG| BAIXA |
|RF-18| A aplicação deve permitir que o usuário acesse uma pagina com todas as ONG's cadastradas| MÉDIA |

### Requisitos não Funcionais

|ID     | Descrição do Requisito  |Prioridade |
|-------|-------------------------|-----------|
|RNF-01| O sistema deve implementar autenticação robusta para proteger contas dos usuários. | ALTA | 
|RNF-02| O sistema deve garantir a privacidade dos dados dos usuários, em conformidade com a LGPD. |  ALTA | 
|RNF-03| O sistema deve possuir interface intuitiva, permitindo que os usuários compreendam e manipulem as funcionalidades com facilidade. |  MÉDIA | 
|RNF-04| O sistema deve oferecer tempos de resposta rápidos, assegurando experiência fluida de uso. |  BAIXA | 

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|01| O projeto deverá ser entregue até o final do semestre.          |
|02| O front-end deve ser desenvolvido usando tecnologias web padrão como HTML, CSS, JavaScript e Bootstrap         |
|03| O backend deve ser implementado utilizando C#.         |
|04| O banco de dados relacional ( SQL Server) deve ser utilizado para implementar no mínimo 2 CRUD's.         |
|05| O desenvolvimento do projeto deve ser realizado com o uso de ferramentas e softwares gratuitos ou com licenças acadêmicas, assegurando que todos os membros da equipe tenham acesso às tecnologias necessárias.          |
|06| Todo o código deve seguir as melhores práticas de codificação e padrões estabelecidos para garantir legibilidade e manutenção.         |
|07| A equipe deve colaborar em todas as etapas do projeto, assegurando que todos os membros estejam envolvidos nas decisões e no desenvolvimento das atividades de forma ativa e participativa.        |
|08| O site deve seguir rigorosamente as diretrizes éticas da instituição, não permitindo a inclusão de conteúdos ofensivos, discriminatórios ou que violem códigos de conduta.         |
|09| O conteúdo do site deve ser original ou proveniente de fontes de domínio público, garantindo a conformidade com as leis de direitos autorais.         |
|10| A aplicação deve estar em conformidade com a Lei Geral de Proteção de Dados (LGPD) do Brasil, garantindo que os dados dos usuários e informações sensíveis estejam protegidos.        |
|11| Todo o material do projeto será disponibilizado em um repositório na plataforma GitHub.        |
|12| A aplicação requer uma conexão constante à internet para funcionar corretamente.         |
|13| A aplicação deverá trazer uma solução para uma ou mais ODS (ONU – Agenda 2030).         |

## Diagrama de Casos de Uso

<img width="1134" height="702" alt="Diagrama de casos de uso" src="https://github.com/user-attachments/assets/8802bbac-8514-45a8-94e3-4a3d4c80016f" />

Figura 01 - Diagrama de casos de uso
