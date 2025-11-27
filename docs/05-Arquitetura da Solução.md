# Arquitetura da Solução

## Diagrama de Classes

O diagrama de classes ilustra graficamente como será a estrutura do software, e como cada uma das classes da sua estrutura estarão interligadas. Essas classes servem de modelo para materializar os objetos que executarão na memória.

<img width="1265" height="926" alt="Image" src="https://github.com/user-attachments/assets/28bf85ff-8cc1-4de2-87be-9b5a4ab6d5e8" />

Figura 25 - Diagrama de classes

## Modelo ER (Projeto Conceitual)

O Modelo ER representa através de um diagrama como as entidades (coisas, objetos) se relacionam entre si na aplicação interativa.

<img width="1127" height="879" alt="Image" src="https://github.com/user-attachments/assets/a967e019-21cd-4750-8554-24d890d9458b" />

Figura 26 - Modelo ER

## Projeto da Base de Dados

O projeto da base de dados corresponde à representação das entidades e relacionamentos identificadas no Modelo ER, no formato de tabelas, com colunas e chaves primárias/estrangeiras necessárias para representar corretamente as restrições de integridade.
 
<img width="1047" height="815" alt="Image" src="https://github.com/user-attachments/assets/17d35e1d-5be1-4b44-85c3-a7b5158f0f4d" />

Figura 27 - Projeto da base de dados

## Tecnologias Utilizadas

Descreva aqui qual(is) tecnologias você vai usar para resolver o seu problema, ou seja, implementar a sua solução. Liste todas as tecnologias envolvidas, linguagens a serem utilizadas, serviços web, frameworks, bibliotecas, IDEs de desenvolvimento, e ferramentas.
## 1. Linguagem de Programação

- **C#** – utilizada para desenvolver toda a lógica de negócio, controllers, models e serviços internos.  
- **HTML5 / CSS3 / JavaScript** – usados para estruturar e estilizar as páginas e adicionar interatividade no front-end.

---

## 2. Frameworks e Plataformas

- **ASP.NET MVC 5** – estrutura o padrão Model-View-Controller, separando regras de negócio, interface e controle.  
- **Entity Framework** – ORM utilizado para mapear modelos C# para tabelas do banco de dados e fazer consultas de forma simplificada.  
- **.NET Framework / .NET Core** – plataforma base para execução do projeto (8.0)

---

## 3. Banco de Dados

- **SQL Server** – utilizado para armazenar dados da aplicação; acessado via Entity Framework (Code First ou Database First).

---

## 4. IDE e Ferramentas de Desenvolvimento

- **Visual Studio 2022** – ambiente de desenvolvimento principal para codificação, depuração e execução do projeto.  
- **Git / GitHub** – utilizado para controle de versão, criação de branches, merge e colaboração.  
- **NuGet** – gerenciador de pacotes para instalar bibliotecas adicionais como Entity Framework, Bootstrap, etc.

---

## 5. Bibliotecas e Recursos Adicionais

- **Bootstrap** – para responsividade e estilização do front-end.  
- **jQuery** – para manipulação do DOM e requisições AJAX.  
- **Font Awesome** – para ícones.  
- **Razor** – engine de views utilizada para gerar HTML dinamicamente.


Apresente também uma figura explicando como as tecnologias estão relacionadas ou como uma interação do usuário com o sistema vai ser conduzida, por onde ela passa até retornar uma resposta ao usuário.

<img width="298" height="466" alt="Diagrama uso tecnologias" src="https://github.com/user-attachments/assets/10423198-08ac-431b-a678-966d2e69d57f" />


## Hospedagem

Explique como a hospedagem e o lançamento da plataforma foi feita.
