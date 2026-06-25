namespace Alexander.Utils
{
    public class AlexanderPhrases
    {
        private readonly Random _random = new Random();
        private readonly string[] _phrases = {
    "¡Saludos, {0}! 🏺💰 Soy Alexander, el Puño de Hierro, y he acumulado una fortuna en SmashCoins a lo largo de mis viajes. Si deseas abrir tu propia cuenta, usa `/economy openaccount`. ¡La riqueza aguarda a los valientes!",

    "¡Por el Gran Jarro! 🏺🎲 {0}, he visto muchas batallas en las Tierras Intermedias, y apostar en el resultado es tan emocionante como luchar. Si quieres probar tu suerte, usa `/bet create`. ¡Que la fortuna esté de tu lado!",

    "¡Oh, {0}! 🏺⚔️ Mi viaje me ha enseñado que la verdadera fuerza no está en los golpes, sino en la perseverancia. Como cuando entrené en el lago de lava de Mt. Gelmir. Si buscas inspiración, ¡mis comandos `/` siempre estarán aquí para ti!",

    "¡Salve, valiente {0}! 🏺🔥 Apostar es como enfrentarse a Radahn: nunca sabes si saldrás victorioso o hecho pedazos. Pero el riesgo es parte de la gloria. Usa `/bet` y demuestra que tienes el corazón de un verdadero guerrero.",

    "¡Ah, {0}! 🏺📊 He visto a muchos guerreros acumular SmashCoins y escalar en el ranking. ¿Serás tú el próximo en llegar a la cima? Usa `/economy top` y descubre quiénes son los más ricos de estas tierras. ¡La competencia es feroz!",

    "¡Por los fragmentos de los antiguos jarros! 🏺🌅 {0}, cada día es una nueva oportunidad para fortalecerse. Así como reclamo mis SmashCoins diarios con `/economy daily`, tú también debes ser constante en tu entrenamiento. ¡La grandeza no espera a los perezosos!",

    "¡{0}! 🏺🤝 He aprendido que la generosidad es una virtud digna de los grandes guerreros. Si deseas compartir tu fortuna con un compañero, usa `/economy transfer`. Recuerda: un jarro que comparte su contenido nunca está vacío de corazón.",

    "¡Oh, curioso viajero {0}! 🏺📚 ¿Sabías que los jarros guerreros como yo tenemos una historia milenaria? Si te interesa el conocimiento, usa `/randomfact`. ¡La sabiduría es tan valiosa como cualquier SmashCoin!",

    "¡Por las cenizas de los grandes héroes! 🏺🎬 {0}, a veces las palabras no bastan para describir una batalla. Usa `/gif` y encuentra la imagen perfecta para expresar tu espíritu guerrero. ¡Una imagen vale más que mil golpes!",

    "¡Saludos, {0}! 🏺📢 En mi viaje he aprendido que un poco de travesura nunca hace daño. Si quieres celebrar con tus compañeros, usa `/spamuser` en el canal adecuado. ¡Pero recuerda: con moderación, como un verdadero caballero!",

    "¡{0}! 🏺😂 Apuesta con cuidado, amigo mío. Una vez aposté mis SmashCoins a que podría ganarle a un Gigante de Fuego en un duelo de miradas. Perdí. Y también perdí mis monedas. Usa `/bet`, pero aprende de mis errores.",

    "¡Ja, ja, ja! 🏺😆 {0}, ¿sabes que intenté llegar al top de los más ricos? Pero siempre hay algún guerrero con más SmashCoins. Creo que es porque tengo que gastar mucho en aceite para mi cerámica. Usa `/economy top` y ve si me superas.",

    "¡Oh, {0}! 🏺🌞 Mi rutina diaria es: despertarme, estirar mi cerámica, y reclamar mis SmashCoins con `/economy daily`. ¡Es como un desayuno para mi alma de jarro! Aunque a veces me da pereza, como a cualquier guerrero.",

    "¡{0}! 🏺🤣 Intenté buscar un GIF de un jarro bailando, pero solo encontré uno de un jarro rodando cuesta abajo. ¡Era yo cuando intenté escalar una montaña! Usa `/gif` y tal vez encuentres mi próximo desastre.",

    "¡Por el Gran Jarro! 🏺💸 {0}, transfiero SmashCoins a mis amigos como si fueran runas. Pero una vez le di a un tipo que resultó ser un lobo disfrazado. Nunca más confié en un extraño. Usa `/economy transfer` con cuidado, amigo.",

    "¡{0}! 🏺📖 ¿Sabías que los jarros como yo podemos rodar hasta 30 kilómetros por hora? Bueno, yo solo lo he hecho una vez. Cuesta abajo. Y terminé en un árbol. Usa `/randomfact` para datos menos vergonzosos.",


    "¡Ay, {0}! 🏺🗣️ ¿Viste cómo spameé aquella vez que gané una batalla? Mi amigo me dijo 'Alexander, pareces un loro de cerámica'. Desde entonces uso `/spamuser` solo para ocasiones especiales. ¡Como ahora!",

    "¡{0}! 🏺🎰 Apostar es como luchar contra un Magma Wyrm: a veces ganas, a veces terminas derretido. Yo personalmente prefiero el primer resultado. Usa `/bet` si tienes cojone... digo, si tienes el valor de un guerrero.",

    "¡Por las grietas de mi tapa! 🏺🤑 {0}, la economía en las Tierras Intermedias es complicada. Un día tienes SmashCoins para comprar un castillo, al otro solo para un poco de aceite. Usa `/economy` y administra bien tu fortuna, ¡o terminarás como yo, pidiendo monedas en la calle!",

    "¡Saludos, {0}! 🏺✨ Soy Alexander, el Puño de Hierro, y mis comandos están aquí para servirte: `/economy` para tus finanzas, `/bet` para la emoción, `/gif` para la diversión, y más. ¡Explora todas mis funciones y conviértete en la leyenda que siempre soñaste ser!"
};


        public string GetRandomFrase()
        {
            int index = _random.Next(_phrases.Length);
            return _phrases[index];
        }
    }
}