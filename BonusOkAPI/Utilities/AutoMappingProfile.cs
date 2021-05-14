using System;
using AutoMapper;
using BonusOkAPI.Contracts;
using BonusOkAPI.Models;

namespace BonusOkAPI.Utilities
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<Client, ClientResponse>();
            CreateMap<Card, CardResponse>();
            CreateMap<Promo, PromoResponse>();
            CreateMap<ClientRequest, Client>();
            CreateMap<CardRequest, Card>();
            CreateMap<PromoRequest, Promo>();
        }
    }
}