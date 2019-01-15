using UnityEngine;
using System.Collections;

//Classe responsável pelo controle dos objetos do tipo Missile

//Requisita a existência de um componente do tipo CharacterController para o GameObject da nave principal
//http://docs.unity3d.com/Documentation/ScriptReference/CharacterController.html
[RequireComponent( typeof( CharacterController ) )]

public class Missile : MonoBehaviour {
	
	//--->Variáveis
	//Faz o link com uma lista de prefab (modelos) de detonadores
	public Transform[] detonator;
	//Variável do tipo CharacterController (componente)
	private CharacterController controller;
	//Guardará uma referência para o objeto
	private GameObject player;
	//Variável do tipo SpaceShipController (script)
	private SpaceShipController controllerSS;
	//Variáveis que controlarão o funcionamento do Hunter, a máquina de estados
	public float speed;
	public float chaseRange;
	public float dieRange;
	
	//--->Função utilizada para inicialização
	void Start()
	{
		//Obtém o componente CharacterController da nave à qual este script está atado
    	controller = GetComponent<CharacterController>();
		//Obtém uma refeência para a nave do jagador
		player = GameObject.Find("RustyFighter");
		//Obtém o componente SpaceShipController da nave do jogador
		controllerSS = player.GetComponent<SpaceShipController> ();
	}
	
	//--->Função chamada quando um objeto do tipo corpo rígido colide com outro corpo rígido
	//http://docs.unity3d.com/Documentation/ScriptReference/Collider.OnCollisionEnter.html
	void Update()
	{	
		//Se a referência para a nave do jogador for nula...
    	if (player == null)
			//Simplesmente retorna-se do método
        	return;
		
		//Obtém-se a distância entre o jogoador e a nave inimiga em questão
    	float range = Vector3.Distance(player.transform.position, transform.position);
		//Se esta distância é menor do que a distância na qual o jogador é considerado
		//atingido
    	if (range <= dieRange)
		{
			//Decrementa-se a quantidade de vida do jogador
			controllerSS.decreaseLives ();
			//Chama-se a corrotina DestroyCountdown, passando o segundo tipo de detonador
			StartCoroutine (DestroyCountdown (detonator[1]));
    	}
		//Se a distância é menor ou igual a distância de alcance do caçador...
    	else if (range <= chaseRange)
    	{
			//Então faz-se o caçador perseguir o jogador
        	transform.LookAt(player.transform);
        	Vector3 moveDirection = transform.TransformDirection(Vector3.forward);
        	controller.Move(moveDirection * speed * Time.deltaTime);
    	}	
		else if (range >= dieRange)
		{
			//Fazer alguma animacao
		}
	}
	
	//--->Função chamada quando um objeto do tipo corpo rígido colide com outro corpo rígido
	//http://docs.unity3d.com/Documentation/ScriptReference/Collider.OnCollisionEnter.html
	void OnCollisionEnter (Collision collision) 
	{
		//Se houve uma colisão com uma bala atirada pelo jogador
		if (collision.gameObject.name == "Bullet(Clone)") 
		{
			//Chama-se a corrotina DestroyCountdown, passando o primeiro tipo de detonador
			StartCoroutine (DestroyCountdown (detonator[0]));
		}
		//Se houve uma colisão com um asteróide...
		else if (collision.gameObject.name == "Asteroid")
		{
			//Chama-se a corrotina DestroyCountdown, passando o primeiro tipo de detonador
			StartCoroutine (DestroyCountdown (detonator[0]));
		}
		//Se houve uma colisão com uma das barreiras do mundo
		else if (collision.gameObject.name == "Left" || collision.gameObject.name == "Right" || collision.gameObject.name == "Down") {
			//Destrói-se este objeto
			Destroy (gameObject);	
		}
	}
	
	//--->Função responsável por simular a destruição do míssil
	private IEnumerator DestroyCountdown (Transform det_)
	{
		//Obtém-se a posição da nave inimiga 
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		//Destrói-se a instância do jogador
		Destroy (gameObject);
		//Cria uma exeplosão a partir do prefab de um detonador existente na variável detonator
		Transform det = (Transform) Instantiate (det_, new Vector3 (x, y, z), Quaternion.identity);
		yield return new WaitForSeconds( 2 );
		//Destrói-se o detonador
		Destroy (det);	
	}
}