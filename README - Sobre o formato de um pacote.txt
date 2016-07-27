O formato de um pacote enviado pela internet deverá ser o seguinte para garantir a integridade de todas as conexões realizadas.

Pacote = TipoDaConexão|Elemento$Elemento'|
Elemento = 'Parâmetro'|'Parâmetro'

O fim de um elemento do pacote é marcado por um $. A string será separada em função das posições destes.
Parâmetros dentro de um mesmo elemento devem ser separados por | pela mesma razão. Assim, no caso da autenticação de um usuário, o pacote deve ter o seguinte formato:

Authenticate|username=Test|passhash=098F6BCD4621D373CADE4E832627B4F6|

Que no servidor produz uma tabela com
Type = Authenticate
username - Test
passhash - 098F6BCD4621D373CADE4E832627B4F6

Abaixo seguem os pacotes mais utilizados para referência.
(Cliente -> Servidor)
Authenticate|'username=Test'|passhash=098F6BCD4621D373CADE4E832627B4F6|

(Servidor -> Cliente)
Authenticate$'success'|'true'$