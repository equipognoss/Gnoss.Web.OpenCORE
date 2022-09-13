![](https://content.gnoss.ws/imagenes/proyectos/personalizacion/7e72bf14-28b9-4beb-82f8-e32a3b49d9d3/cms/logognossazulprincipal.png)

# Gnoss.Web.OpenCORE
 
![](https://github.com/equipognoss/Gnoss.Web.OpenCORE/workflows/BuildWeb/badge.svg)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=bugs)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.OpenCORE&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.OpenCORE)

Es la aplicación principal de la plataforma GNOSS. Se encarga de gestionar la autorización de los usuarios a las páginas de la plataforma, la navegación por la web, mantener la sesión del usuario, la carga del menú de la aplicación web con las opciones que el usuario tiene disponibles, etc. 

Configuración estandar de esta aplicación en el archivo docker-compose.yml: 


```yml
web:
    image: gnoss/gnoss.web.opencore 
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
     scopeIdentity: ${scopeIdentity}
     clientIDIdentity: ${clientIDIdentity}
     clientSecretIdentity: ${clientIDIdentity}
    volumes:
      - ./logs/web:/app/logs
      - ./logs/web:/app/trazas
      - ./ViewsAdministracion:/app/ViewsAdministracion
```

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE

## Código de conducta
Este proyecto a adoptado el código de conducta definido por "Contributor Covenant" para definir el comportamiento esperado en las contribuciones a este proyecto. Para más información ver https://www.contributor-covenant.org/

## Licencia
Este producto es parte de la plataforma [Gnoss Semantic AI Platform Open Core](https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE), es un producto open source y está licenciado bajo GPLv3.



