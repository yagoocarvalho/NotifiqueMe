O formato de um pacote enviado pela internet dever� ser o seguinte para garantir a integridade de todas as conex�es realizadas.

Pacote = TipoDaConex�o|Elemento$Elemento'|
Elemento = 'Par�metro'|'Par�metro'

O fim de um elemento do pacote � marcado por um $. A string ser� separada em fun��o das posi��es destes.
Par�metros dentro de um mesmo elemento devem ser separados por | pela mesma raz�o. Assim, no caso da autentica��o de um usu�rio, o pacote deve ter o seguinte formato:

Authenticate|username=Test|passhash=098F6BCD4621D373CADE4E832627B4F6|

Que no servidor produz uma tabela com
Type = Authenticate
username - Test
passhash - 098F6BCD4621D373CADE4E832627B4F6

Abaixo seguem os pacotes mais utilizados para refer�ncia.
(Cliente -> Servidor)
Authenticate|'username=Test'|passhash=098F6BCD4621D373CADE4E832627B4F6|

(Servidor -> Cliente)
Authenticate$'success'|'true'$