USE [Clinica]
GO
/****** Object:  Table [dbo].[Agendamento]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agendamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdTipoAtendimento] [int] NOT NULL,
	[IdPaciente] [int] NOT NULL,
	[DataHora] [datetime] NOT NULL,
	[DataHoraConfirmacao] [datetime] NULL,
	[EstevePresente] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clinica]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clinica](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](200) NOT NULL,
	[Telefone] [varchar](15) NOT NULL,
	[Endereco] [varchar](200) NOT NULL,
	[Numero] [decimal](6, 0) NOT NULL,
	[Complemento] [varchar](100) NULL,
	[Bairro] [varchar](100) NOT NULL,
	[Cidade] [varchar](100) NOT NULL,
	[Estado] [varchar](50) NOT NULL,
	[Pais] [varchar](20) NOT NULL,
	[CEP] [varchar](15) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Especialidade]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Especialidade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](200) NOT NULL,
	[Tipo] [varchar](150) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medico]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medico](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](200) NOT NULL,
	[CPF] [varchar](14) NOT NULL,
	[RG] [varchar](13) NOT NULL,
	[Telefone] [varchar](15) NOT NULL,
	[Email] [varchar](200) NULL,
	[Endereco] [varchar](200) NOT NULL,
	[Numero] [varchar](10) NOT NULL,
	[Complemento] [varchar](100) NULL,
	[Bairro] [varchar](100) NOT NULL,
	[Cidade] [varchar](100) NOT NULL,
	[Estado] [varchar](50) NOT NULL,
	[Pais] [varchar](20) NOT NULL,
	[CEP] [varchar](9) NOT NULL,
	[DataNascimento] [date] NOT NULL,
	[Especializacao] [varchar](200) NULL,
	[CRM] [varchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Paciente]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Paciente](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](200) NOT NULL,
	[CPF] [varchar](14) NOT NULL,
	[RG] [varchar](13) NOT NULL,
	[Telefone] [varchar](15) NOT NULL,
	[Email] [varchar](200) NULL,
	[Endereco] [varchar](200) NOT NULL,
	[Numero] [varchar](10) NOT NULL,
	[Complemento] [varchar](100) NULL,
	[Bairro] [varchar](100) NOT NULL,
	[Cidade] [varchar](100) NOT NULL,
	[Estado] [varchar](50) NOT NULL,
	[Pais] [varchar](20) NOT NULL,
	[CEP] [varchar](9) NOT NULL,
	[DataNascimento] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoAtendimento]    Script Date: 27/01/2025 14:50:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoAtendimento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdClinica] [int] NOT NULL,
	[IdEspecialidade] [int] NOT NULL,
	[IdMedico] [int] NOT NULL,
	[Descricao] [varchar](500) NOT NULL,
	[Valor] [decimal](7, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Agendamento] ADD  DEFAULT ((0)) FOR [EstevePresente]
GO
ALTER TABLE [dbo].[Agendamento]  WITH CHECK ADD  CONSTRAINT [fk_Agendamento_Paciente] FOREIGN KEY([IdPaciente])
REFERENCES [dbo].[Paciente] ([Id])
GO
ALTER TABLE [dbo].[Agendamento] CHECK CONSTRAINT [fk_Agendamento_Paciente]
GO
ALTER TABLE [dbo].[Agendamento]  WITH CHECK ADD  CONSTRAINT [fk_Agendamento_TipoAtendimento] FOREIGN KEY([IdTipoAtendimento])
REFERENCES [dbo].[TipoAtendimento] ([Id])
GO
ALTER TABLE [dbo].[Agendamento] CHECK CONSTRAINT [fk_Agendamento_TipoAtendimento]
GO
ALTER TABLE [dbo].[TipoAtendimento]  WITH CHECK ADD  CONSTRAINT [fk_TipoAtendimento_Clinica] FOREIGN KEY([IdClinica])
REFERENCES [dbo].[Clinica] ([Id])
GO
ALTER TABLE [dbo].[TipoAtendimento] CHECK CONSTRAINT [fk_TipoAtendimento_Clinica]
GO
ALTER TABLE [dbo].[TipoAtendimento]  WITH CHECK ADD  CONSTRAINT [fk_TipoAtendimento_Especialidade] FOREIGN KEY([IdEspecialidade])
REFERENCES [dbo].[Especialidade] ([Id])
GO
ALTER TABLE [dbo].[TipoAtendimento] CHECK CONSTRAINT [fk_TipoAtendimento_Especialidade]
GO
ALTER TABLE [dbo].[TipoAtendimento]  WITH CHECK ADD  CONSTRAINT [fk_TipoAtendimento_Medico] FOREIGN KEY([IdMedico])
REFERENCES [dbo].[Medico] ([Id])
GO
ALTER TABLE [dbo].[TipoAtendimento] CHECK CONSTRAINT [fk_TipoAtendimento_Medico]
GO
