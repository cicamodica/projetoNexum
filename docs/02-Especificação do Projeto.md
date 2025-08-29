# Especificações do Projeto

## Personas
 <table>
  <tr>
    <th>Persona 01</th>
    <th>Nome</th>
    <th>Perfil</th>
    <th>Motivações</th>
    <th>Desafios</th>
    <th>Como a plataforma pode ajudar</th>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/cb6658d0-85b6-4332-9158-380892a8fa80" width="2000"/></td>
    <td><b>Lucas Almeida</b></td>
    <td>Voluntário e designer gráfico freelancer. </td>
    <td>Quer oferecer suas habilidades para causas sociais, contribuir com impacto real e ter reconhecimento formal do seu trabalho     voluntário.</td>
    <td>Tem pouco tempo disponível, precisa conciliar atividades com o trabalho e não sabe onde encontrar ONGs que necessitem de sua competência.</td>
    <td>Permite cadastrar suas habilidades e disponibilidade, receber notificações sobre oportunidades compatíveis e obter certificados digitais de reconhecimento pelo trabalho realizado. </td>
  </tr>
  <tr>
    <th>Persona 02</th>
    <th>Nome</th>
    <th>Perfil</th>
    <th>Motivações</th>
    <th>Desafios</th>
    <th>Como a plataforma pode ajudar</th>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/bc9c1c09-1889-4c03-ae76-2d2174fc9caf" width="800"/></td>
    <td><b>Mariana Costa</b></td>
    <td>Coordenadora de uma ONG local voltada à educação infantil. </td>
    <td>Quer maximizar o impacto das atividades de sua ONG, encontrar voluntários com habilidades específicas e acessar recursos que outras organizações possam compartilhar. </td>
    <td>Muitas vezes não encontra voluntários qualificados e gasta muito tempo buscando materiais e equipamentos. Precisa de relatórios claros para mostrar resultados a doadores. </td>
    <td>Permite cadastrar suas demandas, encontrar voluntários por competências, compartilhar recursos e gerar relatórios de impacto de forma rápida e confiável.</td>
  </tr>
  <tr>
    <th>Persona 03</th>
    <th>Nome</th>
    <th>Perfil</th>
    <th>Motivações</th>
    <th>Desafios</th>
    <th>Como a plataforma pode ajudar</th>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/a5e8fe1f-1ea3-4242-82c6-3c2e4c330919" width="800"/></td>
    <td><b>Instituto Vila Verde </b></td>
    <td>ONG de médio porte focada em meio ambiente</td>
    <td>Otimizar uso de recursos, colaborar com outras ONGs, aumentar eficiência das ações </td>
    <td>Possui equipamentos ociosos; falta de canal seguro para compartilhamento; necessidade de maior transparência para doadores </td>
    <td>Cadastro e disponibilização de recursos para empréstimo ou troca, acesso a voluntários qualificados, geração de relatórios de utilização e impacto </td>
  </tr>
</table>

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`| QUERO/PRECISO ... `FUNCIONALIDADE` |PARA ... `MOTIVO/VALOR`                 |
|--------------------|------------------------------------|----------------------------------------|
|Coordenadora de ONG   | Cadastrar demandas de voluntários           | Conseguir encontrar pessoas qualificadas rapidamente para minhas atividades              |
|Coordenadora de ONG      | Compartilhar equipamentos e materiais                 | Otimizar o uso dos recursos da ONG e ajudar outras instituições próximas  |

Apresente aqui as histórias de usuário que são relevantes para o projeto de sua solução. As Histórias de Usuário consistem em uma ferramenta poderosa para a compreensão e elicitação dos requisitos funcionais e não funcionais da sua aplicação. Se possível, agrupe as histórias de usuário por contexto, para facilitar consultas recorrentes à essa parte do documento.

> **Links Úteis**:
> - [Histórias de usuários com exemplos e template](https://www.atlassian.com/br/agile/project-management/user-stories)
> - [Como escrever boas histórias de usuário (User Stories)](https://medium.com/vertice/como-escrever-boas-users-stories-hist%C3%B3rias-de-usu%C3%A1rios-b29c75043fac)
> - [User Stories: requisitos que humanos entendem](https://www.luiztools.com.br/post/user-stories-descricao-de-requisitos-que-humanos-entendem/)
> - [Histórias de Usuários: mais exemplos](https://www.reqview.com/doc/user-stories-example.html)
> - [9 Common User Story Mistakes](https://airfocus.com/blog/user-story-mistakes/)

## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais que detalham o escopo do projeto.

### Requisitos Funcionais

|   ID   | Descrição do Requisito  | Prioridade |
|-------|-------------------------|------------|
|RF-01| Permitir que o usuário (ONG/Administrador) crie e gerencie seu perfil na plataforma. | ALTA | 
|RF-02| Permitir autenticação de usuários (ONGs/Administrador) por Login e Logout. | ALTA |
|RF-03| Permitir recuperação/redefinição de senha.  | ALTA |
|RF-04| Permitir que ONGs cadastrem oportunidades de voluntariado e recursos disponíveis.  | ALTA |
|RF-05| Permitir que voluntários e doadores pesquisem causas filtrando por necessidades, localização, área de interesse e disponibilidade.  | ALTA |
|RF-06| Permitir a gestão de recursos e inventário para ONGs. | ALTA |
|RF-07|Gerar relatórios de recursos, inventário, vagas disponibilizadas/preenchidas e candidaturas (ONGs).  | ALTA |
|RF-08| Gerar relatórios de ONGs cadastradas, vagas disponibilizadas/preenchidas e candidaturas (Administrador).  | ALTA |
|RF-09| Implementar um sistema de verificação/aprovação para perfis (ONGs).   | ALTA |
|RF-10| Permitir que ONGs solicitem empréstimo de recursos e equipamentos.  | MÉDIA |
|RF-11| Disponibilizar um sistema de notificação por e-mail (voluntários) ou push (ONGs).  | MÉDIA |
|RF-12| Permitir avaliação e feedback de usuários sobre o sistema.   | BAIXA |


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
|04| O banco de dados relacional (como PostgreSQL ou MySQL) deve ser utilizado para implementar no mínimo 2 CRUD's.         |
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

<img width="1260" height="748" alt="image" src="https://github.com/user-attachments/assets/ee0bcac3-3d17-45a8-a6e1-140ee5248a97" />
