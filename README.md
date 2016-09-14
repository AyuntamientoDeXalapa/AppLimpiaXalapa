# AppLimpiaXalapa

La App Limpia Xalapa ayudará a los ciudadanos a saber por donde viene su camión de la basura, y permite reportar incidentes para la mejora del servicio de limpia pública en Xalapa.

## Primeros pasos

- Clona el repositorio: git clone https://github.com/AyuntamientoDeXalapa/AppLimpiaXalapa.git

## Prerequisitos

- Instalación en Windows:
  * Visual Studio 2015 o Xamarin Studio
  * Xamarin 4.1
  * Android SDK
  * Android NDK

  Puedes basarte en la siguiente [liga](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/).

  Para compilar el Proyecto de iOS se require acceso a un Mac con XCode y iOS SDK. Puedes basarte en el siguiente [instructivo](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/connecting-to-mac/).

- Instalación en IOS
  * Xamarin Studio
  * Xamarin
  * Android SDK
  * Android NDK
  * iOS SDK
  * XCode
  
  Puedes basarate en el siguiente [instructivo](https://developer.xamarin.com/guides/ios/getting_started/installation/mac/)

## Correr las apps

Para correr la aplicación de android es necesario generar las llaves:

1. Entrar a https://console.developers.google.com
2. Crear un nuevo proyecto
3. Activar Google Maps API for Android
4. En el archivo AppLimpia/AppLimpia.Droid/Properties/AssemblyInfo.cs cambiar 
```
[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "<TU llave para mapas>")]
```

Las aplicaciones de Windows y iOS no requiren cambios.

### Autores

* **Arturo Soriano**  - *Desarrollador del proyecto* - [arturosm](https://github.com/arturosm)
* **Grigory Evropeytsev**  - *Desarrollador móvil* - [grigorymx](https://github.com/grigorymx)

### Licencia

Este proyecto está liberado bajo la licencia MIT.
