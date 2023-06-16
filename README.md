# Mario

Crear en Unity un videojuego en 2D llamado Mario. El juego es una pequeña recreación del juego
clásico Mario Bros implementando las mecánicas básicas del mismo.

## Escena
Para crear el fondo de escena, importaremos al proyecto el recurso stage1.png, que contiene un
sprite que muestra las plataformas del juego y crearemos a partir de el un GameObject que
deberemos situar en las coordenadas X=0 e Y=0, y para la Z deberemos escoger un valor que haga
que aparezca por detrás de los personajes del juego (para los que reservaremos la coordenada 0 en
Z).

Ajustaremos la visualización a una resolución de 1280x768, que es la adecuada para que el juego se
desarrolle como está previsto.

La escena precisa tener colisionadores para cada rampa, de forma que sujeten a los personajes que
caminan por encima de ellas. Además necesitaremos colisionadores verticales que impidan a Mario
traspasar los extremos derecho e izquierdo de la pantalla. Por razones didácticas, no obstante, estos
colisionadores los iremos colocando a medida que nos hagan falta.

## Mario
Para crear el personaje de Mario tenemos que importar el tilesheet mario_luigi.png, ajustar su
filtrado para que no desdibuje los límites de los píxeles y dividirlo en los frames que contiene.
Luego crearemos el personaje de Mario a partir de los frames que forman la animación de caminar.
Esto nos creará un personaje que siempre ejecuta esta animación, lo cual es incorrecto, pero tiene la
ventaja de que nos permite, con un solo paso, crear el GameObject, una animación, un controlador
de animación, y dejar todo montado y funcionando.

Lo que deseamos en realidad es que Mario comience ejecutando la animación de idle. Pero como
esta animación sólo tiene un frame, si usamos éste para crear el GameObject, Unity no va a crear
una animación ni va a incluir un controlador de animación.

Pero ahora debemos corregir lo que hemos hecho mal y añadir la animación idle, establecerla en el
animador como la animación en la que se inicia y crear una transición hacia la animación walking
gobernada por un parámetro de tipo bool al que llamaremos también walking.

A continuación, añadiremos a Mario un script llamado Mario y un Rigidbody2D. Si probamos
ahora el juego, veremos que Mario cae sin salvación posible.

Debemos poner un colisionador en el suelo al nivel más bajo para evitar que Mario caiga y permitir
que camine por el. Más adelante precisaremos más colisionadores para el resto de las plataformas.

Programaremos en el script Mario el movimiento a izquierda y derecha de personaje principal,
controlado por las flechas izquierda y derecha. La velocidad de movimiento de ser 3f. Una vez que
nos funcione el movimiento en si, añadiremos el ajuste de la animación y el cambio de orientación
de Mario para que mire hacia el lado hacia el que camina.

A continuación programaremos el salto de Mario, al pulsar la tecla espaciadora. La fuerza de salto
puede ser en torno a 6.5f.

Y como siempre pasa en el primer ejercicio con salto, habremos creado una especie de flappy birds,
ya que Mario cae pero puede remontar el “vuelo” al volver a saltar sin tocar el suelo. Debemos
controlar el salto de Mario para que solo pueda saltar si está sobre una plataforma. Además
deberemos controlar que Mario no pueda cambiar su movimiento a izquierda y derecha cuando está
en el aire.

Añadiremos ahora la animación de salto, que debemos crear, al animador y las correspondientes
transiciones que deberemos gobernar desde el script Mario. Deberemos tener en cuenta si Mario
continúa caminando cuando vuelve a caer a tierra o no.

Ahora programaremos el movimiento de la cámara, siguiendo las indicaciones del siguiente
apartado.

Ha llegado el momento de completar los colisionadores de las rampas superiores y los de cada
extremo de la pantalla.

Programar la frenada. Mario no debe dejar de moverse en el mismo momento que se libere la tecla
correspondiente. En su lugar en ese momento debe comenzar a frenar, con un valor de aceleración
de frenada 7.0f, hasta que alcance una velocidad lo suficientemente pequeña, momento en el que su
velocidad se dejará en 0 y se acabará la frenada. Hay que tener en cuenta que si no se hace la
comprobación de que la velocidad es pequeña y paramos a Mario en ese momento, la aceleración
seguiría actuando y Mario empezaría a moverse en sentido contrario al que iba cuando empezó a
frenar. Durante este período de frenada se deberá mostrar la correspondiente animación.

## Cámara
La cámara debe seguir a Mario cuando este se mueve, pero debe tener su movimiento limitado de
forma que nunca se llegue a ver más allá de dónde acaban las plataformas. Este límite está situado
en las coordenadas X -4.3 y 4.3.

## Tortuga
Deberá crearse un prefab para la tortuga al que se añadirá un Rigidbody2D y un script de control
llamado Tortuga. Previamente habremos creado la tortuga como un GameObject en la escena,
incluyendo ya una animación de caminar, creada a partir de los frames adecuados del tilesheet
disponible como recurso.

Las tortugas deben espanearse en los puntos (-10f, 3.5f, 0f) y (10f, 3.5f, 0f).

El comportamiento inicial de las tortugas será el de caminar, hacia la derecha si se ha espaneado en
el punto más a la izquierda, y hacia la izquierda en el caso contrario. Su velocidad será de 1.8 m/s.

Las tortugas no deben pararse al chocar contra las paredes laterales, en cambio, deben continuar
caminando y aparecer en el extremo contrario de la pantalla.

Si dos tortugas tropiezan una con otra, ambas deben darse la vuelta y continuar su camino en
sentido contrario a como llegaron. Para girar se pararán y ejecutarán una animación de giro, una vez
acabada esta continuarán su movimiento y volverán a la animación de caminar.

## Colisión de una tortuga y Mario
Mario debe evitar a las tortugas. Si una de ellas lo alcanza Mario muere y se acaba el juego.

Al morir, Mario ejecuta una animación específica y cae al vacío. Durante este tiempo en el que
sigue visible, no responde a ningún comando del jugador.

## Captura de las tortugas
El objetivo de Mario es capturar y deshacerse de las tortugas. Para ello debe golpear la plataforma
en la que se encuentra la tortuga en el punto en el que esta se encuentra y desde debajo. La tortuga
entonces se dará la vuelta, ejecutando la correspondiente animación y quedará panza arriba. Estando
en este estado la tortura es inofensiva para Mario. Bien al contrario, si ahora Mario la golpea, saldrá
disparada en la dirección en la que éste se movía y caerá hacia abajo hasta desaparecer.
