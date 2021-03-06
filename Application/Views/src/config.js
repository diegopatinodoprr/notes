import convict from 'convict';
import * as fs from 'fs';

// definition du schema de la configuration
var config = convict({
    env: {
        doc: "The application environment.",
        format: ["production", "development", "test"],
        default: "development",
        env: "NODE_ENV"
    },
    port: {
        doc: "The port to bind.",
        format: "port",
        default: 8080,
        env: "PORT",
        arg: "port"
    }
    
});

// charge l'envorimement par rapport a config
var env = config.get('env');
const envFile = './config/' + env + '.json'
if (fs.existsSync(envFile)) {
    config.loadFile(envFile);
}

// valide la configuration
config.validate({ allowed: 'strict' });

module.exports = config;