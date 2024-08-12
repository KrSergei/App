using AppContracts;
using System.Net;

public record ResiveResult(IPEndPoint EndPoint, Message? Message);