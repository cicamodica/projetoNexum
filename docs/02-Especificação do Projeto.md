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
|Ana Clara  | Uma forma de identificar se uma agência é realmente confiável           | Me sentir mais segura ao contratar seus serviços               |
|Ana Clara       | Ter um mecanismo eficiente e rápido de comunicação                 | Que eu possa sanar todas as minhas dúvidas rapidamente |

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
|-------|-------------------------|----|
|RNF-001| A aplicação deve ser responsiva | MÉDIA | 
|RNF-002| A aplicação deve processar requisições do usuário em no máximo 3s |  BAIXA | 

Com base nas Histórias de Usuário, enumere os requisitos da sua solução. Classifique esses requisitos em dois grupos:

- [Requisitos Funcionais
 (RF)](https://pt.wikipedia.org/wiki/Requisito_funcional):
 correspondem a uma funcionalidade que deve estar presente na
  plataforma (ex: cadastro de usuário).
- [Requisitos Não Funcionais
  (RNF)](https://pt.wikipedia.org/wiki/Requisito_n%C3%A3o_funcional):
  correspondem a uma característica técnica, seja de usabilidade,
  desempenho, confiabilidade, segurança ou outro (ex: suporte a
  dispositivos iOS e Android).
Lembre-se que cada requisito deve corresponder à uma e somente uma
característica alvo da sua solução. Além disso, certifique-se de que
todos os aspectos capturados nas Histórias de Usuário foram cobertos.

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|01| O projeto deverá ser entregue até o final do semestre |
|02| Não pode ser desenvolvido um módulo de backend        |


Enumere as restrições à sua solução. Lembre-se de que as restrições geralmente limitam a solução candidata.

> **Links Úteis**:
> - [O que são Requisitos Funcionais e Requisitos Não Funcionais?](https://codificar.com.br/requisitos-funcionais-nao-funcionais/)
> - [O que são requisitos funcionais e requisitos não funcionais?](https://analisederequisitos.com.br/requisitos-funcionais-e-requisitos-nao-funcionais-o-que-sao/)

## Diagrama de Casos de Uso

O diagrama de casos de uso é o próximo passo após a elicitação de requisitos, que utiliza um modelo gráfico e uma tabela com as descrições sucintas dos casos de uso e dos atores. Ele contempla a fronteira do sistema e o detalhamento dos requisitos funcionais com a indicação dos atores, casos de uso e seus relacionamentos. 

As referências abaixo irão auxiliá-lo na geração do artefato “Diagrama de Casos de Uso”.

> **Links Úteis**:
> - [Criando Casos de Uso](https://www.ibm.com/docs/pt-br/elm/6.0?topic=requirements-creating-use-cases)
> - [Como Criar Diagrama de Caso de Uso: Tutorial Passo a Passo](https://gitmind.com/pt/fazer-diagrama-de-caso-uso.html/)
> - [Lucidchart](https://www.lucidchart.com/)
> - [Astah](https://astah.net/)
> - [Diagrams](https://app.diagrams.net/)
