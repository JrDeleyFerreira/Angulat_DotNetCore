using ProEventos.Domain.Identity;

namespace ProEventos.Domain.Entities;

public class Palestrante
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? MiniCurriculo { get; set; }
    public string? ImagemURL { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }

	public int UserId { get; set; }
	public User? User { get; set; }

	public IEnumerable<RedeSocial>? RedesSociais { get; set; }
    public IEnumerable<PalestranteEvento>? PalestrantesEventos { get; set; }
}
