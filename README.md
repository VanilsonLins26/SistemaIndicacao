# 🚀 Desafio Vortex: Sistema de Indicação

Este projeto é uma Single Page Application (SPA) completa que implementa um sistema de cadastro de usuários com pontuação por indicação, desenvolvido como parte do processo seletivo da Vortex.

A aplicação consiste em um back-end construído com ASP.NET Core e um front-end com Angular e banco de dados SQLite.

## ✨ Funcionalidades Principais

* **Cadastro de Usuário:** Formulário completo para registrar novos usuários com nome, e-mail e senha.
* **Validação de Dados:** Validações no front-end (formato de e-mail e senha) e no back-end (regras do Identity).
* **Autenticação via JWT:** Sistema de login seguro que retorna um JSON Web Token (JWT).
* **Página de Perfil Protegida:** Após o registro ou login, o usuário é direcionado para uma página de perfil que exibe seus dados, pontuação atual e um link de indicação único.
* **Lógica de Indicação:** Ao se cadastrar com o link de indicação de um usuário existente, o usuário que indicou ganha 1 ponto em seu score.
* **Persistência de Sessão:** O usuário continua logado mesmo após atualizar a página (F5), graças a um fluxo de revalidação de token no back-end.
* **Design Responsivo:** A interface se adapta a diferentes tamanhos de tela, de desktops a celulares.

## 🛠️ Tecnologias Utilizadas e Justificativas

### Back-end

* **ASP.NET Core 9:** Framework robusto, de alta performance e multi-plataforma da Microsoft para a construção de APIs RESTful.
* 
* **ASP.NET Core Identity:**
    * **Justificativa:** Em vez de criar um sistema de autenticação manual, optei por usar o Identity.O Identity é um framework mantido pela Microsoft completo de autenticação e autorização, ele fornece toda a base para criar, autenticar, autorizar e gerenciar contas de usuário em uma aplicação web ou API, sem precisar implementar tudo isso manualmente.Ele gerencia de forma testada o hashing de senhas, a estrutura do banco de dados, alem de facilitar o uso do Bearer Token, permitindo focar na lógica de negócio do desafio.
    * 
* **Entity Framework Core com SQLite:**
    * **Justificativa:** Escolhi o SQLite para este desafio pela sua extrema simplicidade e portabilidade. Ele funciona como um único arquivo, eliminando a necessidade de hospedar um banco de dados ou o avaliador instalar ou configurar um servidor de banco de dados, tornando o projeto fácil de executar.
    * 
* **Autenticação JWT Bearer:** Escolhi usar uma autenticação(Bearer Token), para garantir mais segurança e controle de acesso no sistema. Isso faz com que, após o login, o servidor gere um token assinado que identifica o usuário de forma segura e esse token é salvo no localstorage do frontend. Assim, em cada requisição autenticada(como ao acessar a página de perfil ou atualizar os dados do usuário)a API valida o token antes de retornar qualquer informação. Dessa forma, mesmo que a página seja recarregada, o usuário continua autenticado sem precisar refazer o login, e apenas tokens válidos e não expirados têm permissão para acessar dados protegidos. Isso melhora a experiência do usuário e evita acessos indevidos a informações sensíveis.

### Front-end

* **Angular 17:** Framework de front-end robusto para criar SPAs complexas e de alto desempenho.
* **Arquitetura Standalone:** Utilizei a abordagem de componentes standalone, que é a prática mais moderna do Angular, resultando em um código mais limpo e modular, sem a necessidade de `NgModules`.
* **TypeScript:** Garante a segurança de tipagem, reduzindo bugs e facilitando a manutenção e escalabilidade do código.
* **CSS Puro (com Variáveis):** Todo o estilo foi feito com css puro, sem o uso de frameworks de UI, conforme solicitado no desafio.

## 💻 Como Executar o Projeto Localmente (com VS Code)

### Pré-requisitos

Para executar a aplicação, você precisará ter os seguintes softwares instalados:

* **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**: Necessário para rodar a API do back-end.
* **[Node.js](https://nodejs.org/)**: Versão 18 ou superior.
* **[Angular CLI](https://angular.dev/cli)**: `npm install -g @angular/cli`
* **[Visual Studio Code](https://code.visualstudio.com/)**: Editor recomendado para este projeto.

### Passo a Passo

#### 1. Back-end (API)

1.  Abra a pasta do projeto no Visual Studio Code.
2.  Abra o terminal integrado do VS Code no caminho /API (atalho: `Ctrl + '`).
3.  Execute o comando abaixo para iniciar a API. O Entity Framework Core irá criar o banco de dados SQLite (`indicacao.db`) na primeira vez.
    ```bash
    dotnet run --launch-profile https
    ```
4.  A API estará rodando. A URL base é `https://localhost:7066` (a porta pode variar, verifique o terminal). **Deixe este terminal rodando.**

#### 2. Front-end (Angular)

1.  Abra outro terminar na pasta frontEnd(caminho /frontEnd).
4.  Instale as dependências do projeto com o `npm`:
    ```bash
    npm install
    ```
5.  Após a instalação, inicie o servidor de desenvolvimento do Angular:
    ```bash
    ng serve
    ```
6.  Abra seu navegador e acesse **`http://localhost:4200`**.


## ✅ Testando o Fluxo de Indicação (Passo a Passo)

**obs: Geração do código:** código único alfanumérico (8 chars) gerado no momento do cadastro do usuário, atraves de um GUID.

Para verificar a funcionalidade principal do sistema, siga este fluxo de teste:

1.  **Faça o login** com o primeiro usuário de teste que é criado automaticamente:
    * **Email:** `teste1@email.com`
    * **Senha:** `Senha@123`

    > Você será redirecionado para a página de perfil. Note que a **Pontuação** inicial é `0`.

2.  Na página de perfil, **copie o link de indicação** clicando no botão "Copiar".

3.  **Saia da conta** (logout) clicando no botão "Sair"(ou abra em outro navegador).

4.  Em uma nova aba ou na mesma, **cole o link de indicação** que você copiou no seu navegador e acesse. O link será algo como `http://localhost:4200/register?ref=TESTE123`.

    > Você será levado para a página de cadastro.

5.  **Crie uma nova conta** para um usuário inédito. Por exemplo:
    * **Nome:** `Novo Usuário`
    * **Email:** `novo.usuario@email.com`
    * **Senha:** `Senha@123`

6.  Após o cadastro bem-sucedido, você será redirecionado para a página de login. Agora, **faça o login novamente**, mas desta vez com o **primeiro usuário** (`teste1@email.com`).

7. Se você ainda estiver com o primeiro usuario logado em outro navegador,** basta recarregar a página **.

8.  **Verifique a pontuação.**
    > Ao acessar o perfil do usuário `teste1@email.com`, você verá que a **Pontuação** foi atualizada e agora é `1`.

Isso confirma que todo o fluxo de cadastro com indicação e a atualização da pontuação do indicador estão funcionando corretamente.

## 💡 Colaboração com IA

Neste projeto, utilizei uma IA(Gemini) como uma ferramenta de mentoria para acelerar o desenvolvimento e garantir a aplicação das melhores práticas.

* **Planejamento:** A IA foi fundamental na fase inicial para discutir as melhores abordagens, como a decisão de colocar as propriedades "código de ativação e pontuação" dentro do usuário ou em uma tabela separada.

* **Depuração (Debugging):** Durante o desenvolvimento enfrentei alguns bugs, como por exemplo o front end iniciando a pagina totalmente em branco, com a ajuda da IA descobri que estava importando o Router do express ao invés angular/core. Outro erro foi que usei o atributo username do identity para guardar o nome de usuario, mas não conseguia usar nome e sobrenome, a IA me informou que por padrao, o username funciona com um identificador único, não aceitando barra de espaço ou acento, entao criei outro paramento para armazenar o nome completo na classe Usuário.

**Principais Aprendizado:**A interação com a IA me ajudou a descobrir e aplicar novas funcionalidades, como o uso de toasts para exibir notificações ao copiar o link(um recurso que eu não conhecia antes).Além disso, aprendi as melhores práticas para algumas funcionalidades. Foram aprendizados que pretendo aplicar e aprimorar em projetos futuros.
