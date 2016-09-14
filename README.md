# AppLimpiaXalapa

La **App Limpia Xalapa** es un aplicación móvil desarrollada con el fin de ayudar a los ciudadanos a estar más en contacto con los servicios que proporciona el [H. Ayuntamiento de Xalapa, Veracruz](http://www.xalapa.gob.mx) a través de la subdirección de Limpia Pública. Entre sus funcionalidades importantes es la de localizar el camión de la basura asignado a un punto de recolección **montonera**, así mismo reportar aún incidente relacionado con el servicio de recolección de basura.

## Primeros pasos

- Clonar el repositorio: ```git clone https://github.com/AyuntamientoDeXalapa/AppLimpiaXalapa.git```

## Prerequisitos

- Instalación en Windows:
  * Visual Studio 2015 o Xamarin Studio
  * Xamarin 4.1
  * Android SDK
  * Android NDK

  Puedes basarte en la siguiente [liga](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/).

  Para compilar el Proyecto de iOS desde Visual Studio ó Xamarin Studio en Windows, se require acceso a un Mac con XCode y iOS SDK. Puedes basarte en el siguiente [instructivo](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/connecting-to-mac/).

- Instalación en MacOS
  * Xamarin Studio
  * Xamarin
  * Android SDK
  * Android NDK
  * iOS SDK
  * XCode
  
  Puedes basarte en el siguiente [instructivo](https://developer.xamarin.com/guides/ios/getting_started/installation/mac/)

## Correr las apps

Para correr la aplicación de Android es necesario generar las llaves:

1. Entrar a https://console.developers.google.com
2. Crear un nuevo proyecto
3. Activar Google Maps API for Android
4. En el archivo [AppLimpia/AppLimpia.Droid/Properties/AssemblyInfo.cs](../AppLimpia/AppLimpia.Droid/Properties/AssemblyInfo.cs) cambiar
```
[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "<Tu llave para mapas generada en el paso 3>")]
```

Las aplicaciones de Windows y iOS no requiren cambios.

### Autores

* **Arturo Soriano**  - *Desarrollador del proyecto* - [arturosm](https://github.com/arturosm)
* **Grigory Evropeytsev**  - *Desarrollador móvil* - [grigorymx](https://github.com/grigorymx)

### Licencia

Este proyecto está liberado bajo la licencia MIT.
