# PFC3

## Descripción de los programas y la aplicación:

### DeviceCamera

Esta aplicación se utilizará en un celular para capturar las imágenes necesarias para la reconstrucción 3D. 
Se conecta con dos programas : Magic Leap AR e Images.

### Magic Leap AR

Este programa se utilizará en los lentes de RA Magic Leap que brindarán la guía para que el usuario capture las imágenes en la posición e inclinación adecuada.
Se conecta con la aplicación DeviceCamera, para conocer la posición en la que se encuentra el celular dentro de la escena y generar un rastro.
  

### Images

Este programa se utilizará en una computadora para recibir las imágenes necesarias para la reconstrucción 3D.
Se conecta con la aplicación DeviceCamera. 


## Comunicación entre los programas y la aplicación:

Los programas y la aplicación se comunicarán a través del protocolo TCP que requerirá que se definan las IPs y los puertos a utilizar.
