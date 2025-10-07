using System;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;


public class TracoRepository : Repository<Traco>, ITracoRepository
{
    
}
