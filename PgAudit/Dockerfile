# Dockerfile

FROM postgres:13

# przenieś postgresql.conf
COPY postgresql.conf /etc/postgresql/postgresql.conf
COPY pg_hba.conf /etc/postgresql/pg_hba.conf

# Zainstaluj pgAudit
RUN apt-get update && apt-get install -y postgresql-13-pgaudit

# po zainstalowaniu wtyczki włącz ją w postgresql.conf
RUN echo "shared_preload_libraries = 'pgaudit'" >> /etc/postgresql/postgresql.conf
RUN echo "pgaudit.log = 'all'" >> /etc/postgresql/postgresql.conf
RUN echo "pgaudit.log_level = 'log'" >> /etc/postgresql/postgresql.conf
RUN echo "pgaudit.log_parameter = on" >> /etc/postgresql/postgresql.conf
