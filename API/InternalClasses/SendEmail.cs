﻿using API.Model;
using System.Net;
using System.Net.Mail;

namespace API.InternalClasses
{
    internal static class SendEmail
    {
        public static void SendLoginInformation(string email, string password)
        {
            MailAddress from = new(_email, "SkyWhySales");
            MailAddress to = new(email);

            MailMessage mail_message = new(from, to)
            {
                Subject = "Информация для входа в приложение",
                Body = $"<h2>Логин: {email}</h2><br/><h2>Пароль: {password}</h2>",
                IsBodyHtml = true
            };

            SmtpClient smtp = new("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_email, _appPassword),
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            smtp.Send(mail_message);
        }

        public static void SendTicket(Ticket ticket, User user, Flight flight, Airline airline, Airport arrivalairport, Airport departureairport)
        {
            MailAddress from = new(_email, "SkyWhySales");
            MailAddress to = new(user.UEmail);

            MailMessage mail_message = new(from, to)
            {
                Subject = $"Билет №{ticket.TId}",
                Body = $"<!doctype html>\r\n<html lang=\"ru\">\r\n  <head>\r\n    <meta charset=\"UTF-8\" />\r\n    <title>Авиабилет</title>\r\n    <style>\r\n      body {{\r\n        font-family: Arial, sans-serif;\r\n        background-color: #ffffff;\r\n        color: #000000;\r\n        margin: 0;\r\n        padding: 0;\r\n      }}\r\n      .header {{\r\n        background-color: #f0f0f0;\r\n        color: #000000;\r\n        padding: 10px 20px;\r\n        text-align: center;\r\n      }}\r\n      .ticket-info {{\r\n        padding: 20px;\r\n        max-width: 900px;\r\n        margin: 0 auto;\r\n      }}\r\n      .ticket-section {{\r\n        background-color: #ffffff;\r\n        border: 1px solid #cccccc;\r\n        padding: 15px;\r\n        margin-bottom: 20px;\r\n        border-radius: 5px;\r\n      }}\r\n      .flight-title {{\r\n        font-size: 18px;\r\n        font-weight: bold;\r\n        text-align: center;\r\n        margin-bottom: 10px;\r\n      }}\r\n      .flight-detail {{\r\n        display: flex;\r\n        justify-content: space-between;\r\n        padding: 5px 0;\r\n        border-bottom: 1px solid #cccccc;\r\n        width: 100%;\r\n      }}\r\n      .flight-detail:last-child {{\r\n        border-bottom: none;\r\n      }}\r\n      .flight-detail span {{\r\n        flex: 1;\r\n        text-align: center;\r\n        word-wrap: break-word;\r\n        padding: 0 5px;\r\n      }}\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"header\">\r\n      <h1>skywhysales</h1>\r\n      <p>Электронный билет</p>\r\n    </div>\r\n    <div class=\"ticket-info\">\r\n      <div class=\"ticket-section\">\r\n        <p>Номер заказа: {ticket.TId}</p>\r\n        <p>Класс обслуживания: {ticket.TClass}</p>\r\n        <p>Электронный билет:  {ticket.TId}</p>\r\n      </div>\r\n      <div class=\"ticket-section\">\r\n        <p>Пассажир: {user.USurname} {user.UName} {user.UPatronymic}</p>\r\n      </div>\r\n      <div class=\"flight-title\">{departureairport.ApCountry} {departureairport.ApCity} — {arrivalairport.ApCountry} {arrivalairport.ApCity}</div>\r\n      <div class=\"ticket-section\">\r\n        <div class=\"flight-detail\">\r\n          <span>{departureairport.ApName}</span><span>{flight.FDepartureTime}</span\r\n          ><span>{flight.FId}</span><span>{flight.FArrivalTime}</span\r\n          ><span>{arrivalairport.ApName}</span>\r\n        </div>\r\n        <div class=\"flight-detail\">\r\n          <span>{departureairport.ApName}</span>\r\n          <span>{flight.FDepartureTime.Date}</span>\r\n          <span>{(flight.FArrivalTime - flight.FDepartureTime).Hours} часов {(flight.FArrivalTime - flight.FDepartureTime).Minutes} минут</span>\r\n          <span>{airline.AlName}</span>\r\n          <span>{flight.FArrivalTime.Date}</span>\r\n        </div>\r\n      </div>\r\n      <div class=\"ticket-section\">\r\n        <p>Номер билета для регистрации: {ticket.TId}</p>\r\n        <p>Всего в пути: {(flight.FArrivalTime - flight.FDepartureTime).Hours} часов {(flight.FArrivalTime - flight.FDepartureTime).Minutes} минут</p>\r\n        <p>Дата покупки билета: {ticket.TBoughtDate}</p>\r\n        <p>Стоимость: {ticket.TTotalPrice}</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>\r\n",
                IsBodyHtml = true
            };

            SmtpClient smtp = new("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_email, _appPassword),
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            smtp.Send(mail_message);
        }

        private const string _email = "ksebija@gmail.com";
        private const string _appPassword = "brsq xdqx jzce jsqm";

    }
}
