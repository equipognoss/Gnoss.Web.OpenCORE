# Gnoss.Web.OpenCORE

![](https://github.com/equipognoss/Gnoss.Web/workflows/BuildWeb/badge.svg)

Es la aplicación principal de la plataforma GNOSS. Se encarga de gestionar la autorización de los usuarios a las páginas de la plataforma, la navegación por la web, mantener la sesión del usuario, la carga del menú de la aplicación web con las opciones que el usuario tiene disponibles, etc. 


Configuración estandar de esta aplicación en el archivo docker-compose.yml: 

```yml
web:
    image: web
    env_file: .env
    ports:
     - ${puerto_web}:80
    environment:
     acid: ${acid}
     base: ${base}
     oauth: ${oauth}
     virtuosoConnectionString: ${virtuosoConnectionString}
     virtuosoConnectionString_home: ${virtuosoConnectionString_home}
     redis__redis__ip__master: ${redis__redis__ip__master}
     redis__redis__ip__read: ${redis__redis__ip__read}
     redis__redis__bd: ${redis__redis__bd}
     redis__redis__timeout: ${redis__redis__timeout}
     redis__recursos__ip__master: ${redis__recursos__ip__master}
     redis__recursos__ip__read: ${redis__recursos__ip__read}
     redis__recursos__bd: ${redis__recursos__bd}
     redis__recursos__timeout: ${redis__redis__timeout}
     redis__liveUsuarios__ip__master: ${redis__liveUsuarios__ip__master}
     redis__liveUsuarios__ip__read: ${redis__liveUsuarios__ip__read}
     redis__liveUsuarios__bd: ${redis__liveUsuarios__bd}
     redis__liveUsuarios__timeout: ${redis__redis__timeout}
     RabbitMQ__colaReplicacion: ${RabbitMQ}
     RabbitMQ__colaServiciosWin: ${RabbitMQ}
     idiomas: ${idiomas}
     IpServicioSocketsOffline: ${IpServicioSocketsOffline}
     PuertoServicioSocketsOffline: ${puerto_ServicioSocketsOffline}
     Servicios__urlLogin: ${Servicios__urlLogin}
     Servicios__urlFacetas: "http://facetas/CargadorFacetas"
     Servicios__urlResultados: "http://resultados/CargadorResultados"
     Servicios__contextosHome: ${Servicios__contextosHome}
     Servicios__urlFacetas__externo: ${Servicios__urlFacetas__externo}
     Servicios__urlResultados__externo: ${Servicios__urlResultados__externo}
     Servicios__autocompletar: ${Servicios__autocompletar}
     Servicios__etiquetadoAutomatico: ${Servicios__etiquetadoAutomatico}
     Servicios__autocompletarEtiquetas: ${Servicios__autocompletarEtiquetas}
     Servicios__urlInterno: "http://interno"
     Servicios__urlArchivos: "http://archivo/"
     Servicios__urlDocuments: "http://documents/GestorDocumental"
     Servicios__urlContent: ${Servicios__urlContent}
     Servicios__urlStatic: ${Servicios__urlStatic}
     Servicios__urlBase: ${Servicios__urlBase__web}
     dominio: ${dominio}
     https: "true"
     connectionType: ${connectionType}
     Virtuoso__Escritura__VirtuosoLecturaPruebasGnoss_v3: ${Virtuoso__Escritura__VirtuosoLecturaPruebasGnoss_v3}
     Virtuoso__Escritura__VirtuosoLecturaPruebasGnoss_v4: ${Virtuoso__Escritura__VirtuosoLecturaPruebasGnoss_v4}
     BidirectionalReplication__VirtuosoLecturaPruebasGnoss_v3: ${BidirectionalReplication__VirtuosoLecturaPruebasGnoss_v3}
     BidirectionalReplication__VirtuosoLecturaPruebasGnoss_v4: ${BidirectionalReplication__VirtuosoLecturaPruebasGnoss_v4}
     replicacionActivadaHome: "true"
     VirtuosoHome__VirtuosoEscrituraHome: ${VirtuosoHome__VirtuosoEscrituraHome}
     DesplegadoDocker: "true"
    volumes:
      - ./logs/web:/app/logs
      - ./logs/web:/app/trazas
      - ./ViewsAdministracion:/app/ViewsAdministracion
```

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.Platform.Deploy


