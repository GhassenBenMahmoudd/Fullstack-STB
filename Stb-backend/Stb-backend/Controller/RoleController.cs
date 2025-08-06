using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        [HttpGet("permissions")]
        public IActionResult GetUserPermissions()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = "Rôle non trouvé dans le token." });
            }

            var permissions = GetPermissionsByRole(userRole);

            return Ok(new
            {
                UserName = userName,
                Role = userRole,
                RoleDescription = GetRoleDescription(userRole),
                Permissions = permissions,
                Message = $"Permissions pour {userName} ({userRole})"
            });
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetAllRoles()
        {
            var roles = new[]
            {
                new
                {
                    Name = "Manager",
                    Description = "Manager - Accès complet à toutes les fonctionnalités",
                    Permissions = GetPermissionsByRole("Manager")
                },
                new
                {
                    Name = "Employe",
                    Description = "Employé - Peut déclarer des cadeaux et consulter ses déclarations",
                    Permissions = GetPermissionsByRole("Employe")
                },
                new
                {
                    Name = "User",
                    Description = "Utilisateur standard - Accès limité",
                    Permissions = GetPermissionsByRole("User")
                }
            };

            return Ok(new
            {
                Roles = roles,
                Message = "Liste des rôles disponibles"
            });
        }

        [HttpGet("can-declare-gifts")]
        public IActionResult CanDeclareGifts()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            var canDeclare = userRole switch
            {
                "Manager" => true,
                "Employe" => true,
                _ => false
            };

            return Ok(new
            {
                UserName = userName,
                Role = userRole,
                CanDeclareGifts = canDeclare,
                Message = canDeclare 
                    ? $"{userName} peut déclarer des cadeaux" 
                    : $"{userName} ne peut pas déclarer de cadeaux"
            });
        }

        private object GetPermissionsByRole(string role)
        {
            return role switch
            {
                "Manager" => new
                {
                    CanDeclareGifts = true,
                    CanViewAllDeclarations = true,
                    CanUpdateStatus = true,
                    CanArchive = true,
                    CanDelete = true,
                    CanViewReports = true,
                    CanManageUsers = true
                },
                "Employe" => new
                {
                    CanDeclareGifts = true,
                    CanViewAllDeclarations = false,
                    CanUpdateStatus = false,
                    CanArchive = false,
                    CanDelete = false,
                    CanViewReports = false,
                    CanManageUsers = false
                },
                _ => new
                {
                    CanDeclareGifts = false,
                    CanViewAllDeclarations = false,
                    CanUpdateStatus = false,
                    CanArchive = false,
                    CanDelete = false,
                    CanViewReports = false,
                    CanManageUsers = false
                }
            };
        }

        private string GetRoleDescription(string role)
        {
            return role switch
            {
                "Manager" => "Manager - Accès complet à toutes les fonctionnalités",
                "Employe" => "Employé - Peut déclarer des cadeaux et consulter ses déclarations",
                _ => "Utilisateur standard - Accès limité"
            };
        }
    }
} 