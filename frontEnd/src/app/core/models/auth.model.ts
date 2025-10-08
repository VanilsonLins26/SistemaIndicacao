export interface UsuarioDto{
    id:string;
    nome:string;
    email:string;
    pontuacao:number;
    codigoIndicacao:string;
}

export interface LoginResponseDto {
  accessToken: string;
  refreshToken: string;
  expiracao: string;
  usuario: UsuarioDto;
}

export interface LoginDto {
  email: string;
  senha: string;
}

export interface RegistrarDto {
  nome: string;
  email: string;
  senha: string;
}