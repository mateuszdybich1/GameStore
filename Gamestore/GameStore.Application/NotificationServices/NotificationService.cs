using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.NotificationServices;

public class NotificationService(UserManager<PersonModel> userManager, RoleManager<RoleModel> roleManager, Func<RepositoryTypes, IOrderRepository> orderRepositoryFactory, IEmailService emailService) : INotificationService
{
    private readonly UserManager<PersonModel> _userManager = userManager;
    private readonly RoleManager<RoleModel> _roleManager = roleManager;
    private readonly IOrderRepository _sqlOrderRepository = orderRepositoryFactory(RepositoryTypes.Sql);
    private readonly IOrderRepository _mongoOrderRepository = orderRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IEmailService _emailService = emailService;

    public async Task NotifyOrderStatusChange(Guid orderId, string status)
    {
        var order = await _sqlOrderRepository.Get(orderId) ?? await _mongoOrderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Order with id: {orderId} not found");

        var roleModels = _roleManager.Roles.ToList();

        var selectedRoles = new HashSet<RoleModel>();
        foreach (var roleModel in roleModels)
        {
            var allClaims = await _roleManager.GetClaimsAsync(roleModel);
            if (allClaims.Select(x => x.Value).Contains(Permissions.UpdateOrder.ToString()))
            {
                selectedRoles.Add(roleModel);
            }
        }

        var usersToInfrom = new HashSet<PersonModel>();

        foreach (var selectedRole in selectedRoles)
        {
            var managerForRole = await _userManager.GetUsersInRoleAsync(selectedRole.Name.ToString());
            foreach (var manager in managerForRole)
            {
                usersToInfrom.Add(manager);
            }
        }

        var orderCustomer = await _userManager.FindByIdAsync(order.CustomerId.ToString());
        if (orderCustomer != null)
        {
            usersToInfrom.Add(orderCustomer);
        }

        await NotifyUsers(usersToInfrom, order);
    }

    private async Task NotifyUsers(IEnumerable<PersonModel> users, Order order)
    {
        string subject = "Order Status Changed";
        string body = $"Your order {order.Id} status has changed to {order.Status}";
        foreach (var user in users)
        {
            if (user != null)
            {
                if (user.NotificationTypes.Contains(UserNotificationType.Email.ToString()) && user.Email != null)
                {
                    await _emailService.SendEmailRequest(new Dtos.EmailRequestDto(user.Email, user.UserName.ToString(), subject, body));
                }

                if (user.NotificationTypes.Contains(UserNotificationType.Sms.ToString()))
                {
                    // TODO
                }

                if (user.NotificationTypes.Contains(UserNotificationType.Push.ToString()))
                {
                    // TODO
                }
            }
        }
    }
}
