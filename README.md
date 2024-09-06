# Bajista-Virtual
Aplicación de Unity que permite reproducir un archivo MIDI junto a una animación de como tocar una pista de audio elegida con un bajo.

# Estructura del proyecto
El proyecto está compuesto por dos módulos: uno de procesamiento del archivo MIDI escrito en C++, y la parte de la UI y de la animación en Unity escrita en C#.

# Utilizar módulo C++ por separado
Para poder compilar y ejecutar el módulo de procesamiento del archivo MIDI por separado es necesario:
- Descargar JUCE desde la pagina web oficial (https://juce.com/get-juce/download)
- Cambiar la configuración de proyecto de inicio a standalone
- Comentar las líneas de código que ponga Unity Editor o Unity Build y descomentar las que ponga debug

# Realizar nueva Build de Unity del proyecto
Para poder realizar una build en Unity hay que realizar los siguientes pasos:
- Asegurarse de que la .dll del módulo C++ tiene descomentadas las líneas de código de Unity Build (Por defecto estará en Unity Editor)
- Colocar .dll en la carpeta de plugins del módulo de Unity (assets/plugins)