API em C# com ASP.NET

Esta é uma API RESTful desenvolvida em C# utilizando ASP.NET Core, projetada para fornecer funcionalidades escaláveis para gerenciamento de chamados e mensagens internas do sistema TechDeskMobile. A API segue os princípios REST, priorizando desempenho, segurança e integração com clientes mobile, desktop e front-end.


Funcionalidades
	•	Gerenciamento de Chamados: Criação, atualização, consulta e finalização de chamados, incluindo controle de status, título e descrição.
	•	Mensagens Internas: Envio e recebimento de mensagens vinculadas a chamados, permitindo comunicação entre usuários, IA e futuramente com técnicos.
	•	Autenticação e Autorização: Suporte para autenticação de usuários e controle de acesso seguro aos recursos.
	•	Consultas e Filtros: Busca de chamados e mensagens com filtros por status.
	•	Documentação Interativa: Suporte a Swagger para visualização e testes dos endpoints.


Tecnologias Utilizadas
	•	ASP.NET Core: Framework moderno para aplicações web de alto desempenho.
	•	C#: Linguagem principal utilizada para lógica de negócios e manipulação de dados.
	•	SQL Server: Banco de dados relacional utilizado para persistência de dados.
	•	Entity Framework Core: ORM para acesso e manipulação de dados.
	•	JWT Authentication: Segurança e controle de acesso.
	•	Swagger / Swashbuckle: Documentação automatizada da API.
	•	AutoMapper: Mapeamento de objetos e DTOs.
	•	Arquitetura MVC: Organização clara entre Models, Views (para testes ou Swagger) e Controllers.
