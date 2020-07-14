import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, NavLink } from 'react-router-dom'
import './index.css';
import App from './components/App';
import * as serviceWorker from './serviceWorker';
import { Button, Container, Form, InputGroup, Nav, Navbar } from 'react-bootstrap';
import './_custom.scss';

//const store = configureStore();

ReactDOM.render(
  <React.StrictMode>
    {/*<ReduxProvider store={store}>*/}
      <Router>
        <Navbar expand="lg" bg="dark" variant="dark" fixed="top">
          <Container className="justify-content-sm-between flex-sm-row flex-column align-items-sm-center align-items-start">
            <div className="d-inline-block">
              <Navbar.Toggle aria-controls="basic-navbar-nav" />
              <Navbar.Brand href="/" className="pl-2">Mars Rover Photos</Navbar.Brand>
            </div>
            <Navbar.Collapse id="basic-navbar-nav">
              <Nav>
                <NavLink className="nav-link" to="/" exact>Home</NavLink>
                <NavLink className="nav-link" to="/curiosity">Curiosity Photos</NavLink>
                <NavLink className="nav-link" to="/opportunity">Opportunity Photos</NavLink>
                <NavLink className="nav-link" to="/spirit">Spirit Photos</NavLink>
              </Nav>
            </Navbar.Collapse>
            <Form id="search" inline className="py-2 py-sm-0 d-sm-flex d-sm-flex d-inline-block">
              <InputGroup className="flex-nowrap flex-sm-wrap">
                <Form.Control id="search" type="text" aria-label="Rever name and/or date" placeholder="Rover name and/or date" aria-describedby="button-search" />
                <InputGroup.Append>
                  <Button id="button-search" type="submit">
                    <svg width="1em" height="1em" role="img" viewBox="0 0 16 16" className="bi bi-search" style={{paddingBottom: "3px", paddingLeft: "1px"}} fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                      <path fillRule="evenodd" d="M10.442 10.442a1 1 0 0 1 1.415 0l3.85 3.85a1 1 0 0 1-1.414 1.415l-3.85-3.85a1 1 0 0 1 0-1.415z"/>
                      <path fillRule="evenodd" d="M6.5 12a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11zM13 6.5a6.5 6.5 0 1 1-13 0 6.5 6.5 0 0 1 13 0z"/>
                    </svg>
                  </Button>
                </InputGroup.Append>
              </InputGroup>
            </Form>
          </Container>
        </Navbar>
        <App />
      </Router>
    {/*</ReduxProvider>*/}
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
